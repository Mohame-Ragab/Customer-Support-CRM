using FluentValidation;

namespace CustomerSupportCRM.Application.Features.Reports.Tickets;

public sealed class GetTicketReportQueryValidator : AbstractValidator<GetTicketReportQuery>
{
    public GetTicketReportQueryValidator()
    {
        RuleFor(x => x.ToDate).GreaterThanOrEqualTo(x => x.FromDate)
            .WithMessage("'ToDate' must be on or after 'FromDate'.");
        RuleFor(x => x)
            .Must(x => x.ToDate.DayNumber - x.FromDate.DayNumber <= 366)
            .WithMessage("Date range cannot exceed 366 days.");
    }
}
