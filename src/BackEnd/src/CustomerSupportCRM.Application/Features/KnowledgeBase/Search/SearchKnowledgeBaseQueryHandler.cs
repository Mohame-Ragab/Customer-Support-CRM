using System.Globalization;
using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Interfaces;
using FluentValidation;
using MediatR;
using ValidationException = CustomerSupportCRM.Application.Common.Exceptions.ValidationException;

namespace CustomerSupportCRM.Application.Features.KnowledgeBase.Search;

/// <summary>
/// EF Core LINQ against SQL Server (no dedicated search engine - out of scope
/// per the story). Uses plain <c>string.Contains</c> (not
/// <c>EF.Functions.Like</c>) so the Application layer never needs an
/// <c>Microsoft.EntityFrameworkCore</c> reference (Onion layering, same
/// reason ToListAsync is avoided elsewhere) - EF Core's SQL Server provider
/// still translates it to a parameterized <c>LIKE</c>, which is also immune
/// to the wildcard-injection concern the story raised for a raw
/// <c>EF.Functions.Like($"%{q}%")</c> string interpolation.
/// </summary>
public sealed class SearchKnowledgeBaseQueryHandler
    : IRequestHandler<SearchKnowledgeBaseQuery, PagedResult<KnowledgeBaseSearchResultDto>>
{
    private const int CandidateSetSize = 500;
    private const int ExcerptRadius = 120;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<SearchKnowledgeBaseQuery> _validator;

    public SearchKnowledgeBaseQueryHandler(IUnitOfWork unitOfWork, IValidator<SearchKnowledgeBaseQuery> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<PagedResult<KnowledgeBaseSearchResultDto>> Handle(
        SearchKnowledgeBaseQuery request, CancellationToken cancellationToken)
    {
        // Validated explicitly here (not via ValidationFilter) - see the
        // controller's XML doc for why this endpoint binds scalar query
        // parameters instead of the record directly.
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 50);

        var tokens = request.Query
            .Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(t => t.Length >= 2)
            .Distinct()
            .ToList();

        if (tokens.Count == 0)
        {
            // Whitespace-only after trimming (validator already rejects < 2 chars,
            // but guards belt-and-braces against a query made entirely of tokens < 2 chars).
            return new PagedResult<KnowledgeBaseSearchResultDto>([], page, pageSize, 0);
        }

        var query = _unitOfWork.Repository<KnowledgeBaseContent>().Query()
            .Where(c => c.IsPublished);

        if (!string.IsNullOrWhiteSpace(request.Language))
        {
            var language = request.Language.ToLowerInvariant();
            query = query.Where(c => c.Language == language);
        }

        // AND across tokens (every token must appear in Title or Body), OR across fields per token.
        foreach (var token in tokens)
        {
            var t = token;
            query = query.Where(c => c.Title.Contains(t) || c.Body.Contains(t));
        }

        // Bounded candidate set - ranking happens in memory (see class doc for the trade-off).
        var candidates = query
            .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
            .Take(CandidateSetSize)
            .ToList();

        var tieBreakLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();
        var trimmedQuery = request.Query.Trim();

        var scored = candidates
            .Select(c => new
            {
                Content = c,
                Score = Score(c, tokens, trimmedQuery, tieBreakLanguage),
            })
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.Content.UpdatedAt ?? x.Content.CreatedAt)
            .ToList();

        var totalCount = scored.Count;

        var pageItems = scored
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new KnowledgeBaseSearchResultDto(
                x.Content.Id,
                x.Content.Title,
                BuildExcerpt(x.Content.Body, tokens),
                x.Content.Language,
                x.Score,
                x.Content.UpdatedAt ?? x.Content.CreatedAt))
            .ToList();

        return new PagedResult<KnowledgeBaseSearchResultDto>(pageItems, page, pageSize, totalCount);
    }

    private static double Score(
        KnowledgeBaseContent content, IReadOnlyList<string> tokens, string exactQuery, string tieBreakLanguage)
    {
        double score = 0;

        foreach (var token in tokens)
        {
            if (content.Title.Contains(token, StringComparison.OrdinalIgnoreCase))
            {
                score += 10;
            }
            if (content.Body.Contains(token, StringComparison.OrdinalIgnoreCase))
            {
                score += 3;
            }
        }

        if (content.Title.Contains(exactQuery, StringComparison.OrdinalIgnoreCase))
        {
            score += 5;
        }
        if (content.Body.Contains(exactQuery, StringComparison.OrdinalIgnoreCase))
        {
            score += 2;
        }

        if (string.Equals(content.Language, tieBreakLanguage, StringComparison.OrdinalIgnoreCase))
        {
            score += 1;
        }

        return score;
    }

    private static string BuildExcerpt(string body, IReadOnlyList<string> tokens)
    {
        var matchIndex = -1;
        foreach (var token in tokens)
        {
            var index = body.IndexOf(token, StringComparison.OrdinalIgnoreCase);
            if (index >= 0 && (matchIndex == -1 || index < matchIndex))
            {
                matchIndex = index;
            }
        }

        if (matchIndex == -1)
        {
            // No direct match in Body (all tokens matched via Title only) - lead with the start of Body.
            matchIndex = 0;
        }

        var start = Math.Max(0, matchIndex - ExcerptRadius);
        var end = Math.Min(body.Length, matchIndex + ExcerptRadius);
        var excerpt = body[start..end];

        var prefix = start > 0 ? "…" : string.Empty;
        var suffix = end < body.Length ? "…" : string.Empty;
        return $"{prefix}{excerpt}{suffix}";
    }
}
