using FluentValidation;

namespace CustomerSupportCRM.Application.Features.Reports.AgentPerformance;

public sealed class GetAgentPerformanceQueryValidator : AbstractValidator<GetAgentPerformanceQuery>
{
    public GetAgentPerformanceQueryValidator()
    {
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From)
            .WithMessage("'To' must be on or after 'From'.");
        RuleFor(x => x)
            .Must(x => (x.To - x.From).TotalDays <= 366)
            .WithMessage("Date range cannot exceed 366 days.");
        RuleFor(x => x.AgentId).NotEqual(Guid.Empty)
            .When(x => x.AgentId.HasValue)
            .WithMessage("'AgentId' must not be an empty guid.");
    }
}
