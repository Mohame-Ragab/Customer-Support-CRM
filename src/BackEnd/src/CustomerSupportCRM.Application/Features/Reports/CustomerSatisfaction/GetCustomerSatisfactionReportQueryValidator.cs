using FluentValidation;

namespace CustomerSupportCRM.Application.Features.Reports.CustomerSatisfaction;

public sealed class GetCustomerSatisfactionReportQueryValidator : AbstractValidator<GetCustomerSatisfactionReportQuery>
{
    public GetCustomerSatisfactionReportQueryValidator()
    {
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From)
            .WithMessage("'To' must be on or after 'From'.");
        RuleFor(x => x)
            .Must(x => x.To.DayNumber - x.From.DayNumber <= 366)
            .WithMessage("Date range cannot exceed 366 days.");
    }
}
