using MediatR;

namespace CustomerSupportCRM.Application.Features.Reports.Tickets;

/// <summary>
/// F09 reports-management/ticket-reports. Bound from individual scalar query
/// parameters on the controller (not this record directly) so ASP.NET Core's
/// automatic model-state validation never short-circuits before
/// ValidationFilter/FluentValidation runs - same rationale as
/// SearchKnowledgeBaseQuery (F06). The handler validates explicitly via
/// IValidator&lt;GetTicketReportQuery&gt;.
/// </summary>
public sealed record GetTicketReportQuery(DateOnly FromDate, DateOnly ToDate) : IRequest<TicketReportResponse>;

public sealed record TicketReportResponse(
    DateOnly FromDate,
    DateOnly ToDate,
    int TotalTickets,
    IReadOnlyList<TicketReportBucket> ByStatus,
    IReadOnlyList<TicketReportBucket> ByCategory,
    IReadOnlyList<TicketReportBucket> ByPriority);

public sealed record TicketReportBucket(string Key, int Count);
