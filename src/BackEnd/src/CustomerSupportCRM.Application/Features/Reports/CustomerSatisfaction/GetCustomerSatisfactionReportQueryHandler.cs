using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Interfaces;
using FluentValidation;
using MediatR;
using ValidationException = CustomerSupportCRM.Application.Common.Exceptions.ValidationException;

namespace CustomerSupportCRM.Application.Features.Reports.CustomerSatisfaction;

/// <summary>Aggregates CustomerFeedback (F08) over a date range. Never 404/empty-error - a period with no feedback returns a zero-valued 200.</summary>
public sealed class GetCustomerSatisfactionReportQueryHandler
    : IRequestHandler<GetCustomerSatisfactionReportQuery, CustomerSatisfactionReportDto>
{
    private static readonly int[] ValidRatings = [1, 2, 3, 4, 5];

    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<GetCustomerSatisfactionReportQuery> _validator;

    public GetCustomerSatisfactionReportQueryHandler(
        IUnitOfWork unitOfWork, IValidator<GetCustomerSatisfactionReportQuery> validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<CustomerSatisfactionReportDto> Handle(
        GetCustomerSatisfactionReportQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var fromUtc = request.From.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var toExclusiveUtc = request.To.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

        var feedback = _unitOfWork.Repository<CustomerFeedback>().Query()
            .Where(f => f.SubmittedAt >= fromUtc && f.SubmittedAt < toExclusiveUtc)
            .ToList();

        // Every valid rating key is present, even zero-count, so downstream
        // chart code never sees a missing bucket.
        var distribution = ValidRatings.ToDictionary(r => r, _ => 0);
        foreach (var item in feedback)
        {
            if (distribution.ContainsKey(item.Rating))
            {
                distribution[item.Rating]++;
            }
            // Ratings outside the declared 1-5 scale should not occur (enforced by
            // F08's SubmitFeedbackRequestValidator) - silently excluded from the
            // distribution rather than throwing, per this story's edge-case notes.
        }

        var totalResponses = feedback.Count;
        var averageRating = totalResponses == 0 ? 0d : Math.Round(feedback.Average(f => f.Rating), 2);
        var satisfactionScorePercent = totalResponses == 0 ? 0d : Math.Round((averageRating - 1) / 4 * 100, 2);

        return new CustomerSatisfactionReportDto(
            request.From, request.To, totalResponses, averageRating, satisfactionScorePercent, distribution);
    }
}
