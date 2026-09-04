using MediatR;

namespace CustomerSupportCRM.Application.Features.Reports.CustomerSatisfaction;

/// <summary>F09 reports-management/customer-satisfaction-reports. Scalar-bound - see GetTicketReportQuery for the ValidationFilter rationale.</summary>
public sealed record GetCustomerSatisfactionReportQuery(DateOnly From, DateOnly To)
    : IRequest<CustomerSatisfactionReportDto>;

/// <summary>
/// Rating scale is 1-5, fixed by CustomerFeedback.Rating's own validator
/// (F08 customer-portal/submit-customer-feedback). SatisfactionScorePercent
/// normalizes AverageRating onto 0-100: (AverageRating - 1) / 4 * 100.
/// </summary>
public sealed record CustomerSatisfactionReportDto(
    DateOnly PeriodStart,
    DateOnly PeriodEnd,
    int TotalResponses,
    double AverageRating,
    double SatisfactionScorePercent,
    IReadOnlyDictionary<int, int> RatingDistribution);
