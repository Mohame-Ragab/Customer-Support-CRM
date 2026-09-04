using FluentValidation;

namespace CustomerSupportCRM.Application.Features.CustomerPortal.Tickets;

public sealed class SubmitPortalTicketCommandValidator : AbstractValidator<SubmitPortalTicketCommand>
{
    public SubmitPortalTicketCommandValidator()
    {
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(4000);
    }
}
