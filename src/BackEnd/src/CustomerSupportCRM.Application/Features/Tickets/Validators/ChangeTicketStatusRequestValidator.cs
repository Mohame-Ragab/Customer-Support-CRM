using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using FluentValidation;

namespace CustomerSupportCRM.Application.Features.Tickets.Validators;

public sealed class ChangeTicketStatusRequestValidator : AbstractValidator<ChangeTicketStatusRequest>
{
    public ChangeTicketStatusRequestValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}
