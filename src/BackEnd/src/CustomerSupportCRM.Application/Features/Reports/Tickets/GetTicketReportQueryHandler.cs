using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Interfaces;
using FluentValidation;
using MediatR;
using ValidationException = CustomerSupportCRM.Application.Common.Exceptions.ValidationException;

namespace CustomerSupportCRM.Application.Features.Reports.Tickets;

/// <summary>Read-only ticket volume/status/category/priority aggregation over a date range (F09 ticket-reports). No persistence, no background jobs.</summary>
public sealed class GetTicketReportQueryHandler : IRequestHandler<GetTicketReportQuery, TicketReportResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<GetTicketReportQuery> _validator;

    public GetTicketReportQueryHandler(IUnitOfWork unitOfWork, IValidator<GetTicketReportQuery> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<TicketReportResponse> Handle(GetTicketReportQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var fromUtc = request.FromDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var toExclusiveUtc = request.ToDate.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

        var tickets = _unitOfWork.Repository<Ticket>().Query()
            .Where(t => t.CreatedAt >= fromUtc && t.CreatedAt < toExclusiveUtc)
            .ToList();

        var categoryIds = tickets.Where(t => t.CategoryId.HasValue).Select(t => t.CategoryId!.Value).Distinct().ToList();
        var categoryCodes = categoryIds.Count == 0
            ? new Dictionary<Guid, string>()
            : (await _unitOfWork.Repository<TicketCategory>().FindAsync(c => categoryIds.Contains(c.Id), cancellationToken))
                .ToDictionary(c => c.Id, c => c.Code);

        var byStatus = tickets
            .GroupBy(t => t.Status.ToString())
            .Select(g => new TicketReportBucket(g.Key, g.Count()))
            .OrderByDescending(b => b.Count)
            .ToList();

        var byCategory = tickets
            .GroupBy(t => t.CategoryId.HasValue && categoryCodes.TryGetValue(t.CategoryId.Value, out var code)
                ? code
                : "Uncategorized")
            .Select(g => new TicketReportBucket(g.Key, g.Count()))
            .OrderByDescending(b => b.Count)
            .ToList();

        var byPriority = tickets
            .GroupBy(t => t.Priority.HasValue ? t.Priority.Value.ToString() : "None")
            .Select(g => new TicketReportBucket(g.Key, g.Count()))
            .OrderByDescending(b => b.Count)
            .ToList();

        return new TicketReportResponse(request.FromDate, request.ToDate, tickets.Count, byStatus, byCategory, byPriority);
    }
}
