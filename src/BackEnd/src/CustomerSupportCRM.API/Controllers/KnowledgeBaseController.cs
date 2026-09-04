using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.KnowledgeBase.Dtos;
using CustomerSupportCRM.Application.Features.KnowledgeBase.Queries;
using CustomerSupportCRM.Application.Features.KnowledgeBase.Search;
using CustomerSupportCRM.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupportCRM.API.Controllers;

/// <summary>
/// Public, read-only Knowledge Base search (F06 knowledge-base/search-knowledge-base).
/// Anonymous by design - both agents and (future) customer-portal visitors use
/// it; only published content is ever returned. Authoring lives in the
/// separate, staff-only <see cref="KnowledgeBaseContentController"/>.
/// </summary>
[ApiController]
[AllowAnonymous]
[Route("api/knowledge-base")]
public sealed class KnowledgeBaseController : BaseApiController
{
    private readonly IMediator _mediator;

    public KnowledgeBaseController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// GET /api/knowledge-base/search?query=&amp;language=&amp;page=&amp;pageSize= - ranked title/body match over published content.
    /// Bound as individual scalar query parameters (not a single [FromQuery]
    /// complex-type parameter) deliberately: binding the whole
    /// SearchKnowledgeBaseQuery record as one [FromQuery] parameter makes
    /// [ApiController]'s automatic model-state validation infer an implicit
    /// "required" check on the non-nullable Query property and short-circuit
    /// with its own ValidationProblemDetails shape *before* ValidationFilter /
    /// FluentValidation ever runs - a different, inconsistent 400 shape from
    /// every other endpoint in this API. The handler validates explicitly via
    /// IValidator&lt;SearchKnowledgeBaseQuery&gt; and throws this app's own
    /// ValidationException instead, so the response shape matches everywhere else.
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResult<KnowledgeBaseSearchResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<KnowledgeBaseSearchResultDto>>> Search(
        [FromQuery] string? query, [FromQuery] string? language,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var searchQuery = new SearchKnowledgeBaseQuery(query ?? string.Empty, language, page, pageSize);
        return Ok(await _mediator.Send(searchQuery, ct));
    }

    /// <summary>
    /// GET /api/knowledge-base/articles?type=&amp;language=&amp;page=&amp;pageSize= - the
    /// published catalog, for browsing without a search term (F08
    /// customer-portal/access-faqs). Reuses the staff GetKnowledgeBaseContentListQuery
    /// unchanged, forcing IsPublished = true regardless of any client input -
    /// there is no way for an anonymous/customer caller to request drafts here.
    /// </summary>
    [HttpGet("articles")]
    [ProducesResponseType(typeof(PagedResult<KnowledgeBaseContentListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<KnowledgeBaseContentListItemDto>>> GetPublishedArticles(
        [FromQuery] ContentType? type, [FromQuery] string? language,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var query = new GetKnowledgeBaseContentListQuery(
            Type: type, Language: language, IsPublished: true, Search: null, Page: page, PageSize: pageSize);
        return Ok(await _mediator.Send(query, ct));
    }

    /// <summary>GET /api/knowledge-base/articles/{id} - a single published article; 404 for drafts or unknown ids.</summary>
    [HttpGet("articles/{id:guid}")]
    [ProducesResponseType(typeof(KnowledgeBaseContentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<KnowledgeBaseContentDto>> GetPublishedArticleById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetPublishedKnowledgeBaseArticleQuery(id), ct));
}
