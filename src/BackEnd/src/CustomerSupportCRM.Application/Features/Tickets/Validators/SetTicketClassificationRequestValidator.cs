using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using FluentValidation;

namespace CustomerSupportCRM.Application.Features.Tickets.Validators;

public sealed class SetTicketClassificationRequestValidator : AbstractValidator<SetTicketClassificationRequest>
{
    public SetTicketClassificationRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x.CategoryId.HasValue || x.Priority.HasValue)
            .WithMessage("Provide a category, a priority, or both.");

        RuleFor(x => x.Priority)
            .IsInEnum()
            .When(x => x.Priority.HasValue);
    }
}
