using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using FluentValidation;

namespace CustomerSupportCRM.Application.Features.Tickets.Validators;

public sealed class AssignTicketRequestValidator : AbstractValidator<AssignTicketRequest>
{
    public AssignTicketRequestValidator()
    {
        RuleFor(x => x.AgentUserId).NotEmpty();
    }
}
