using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using FluentValidation;

namespace CustomerSupportCRM.Application.Features.Tickets.Validators;

public sealed class EscalateTicketRequestValidator : AbstractValidator<EscalateTicketRequest>
{
    public EscalateTicketRequestValidator()
    {
        RuleFor(x => x.Reason).MaximumLength(1000);
    }
}
