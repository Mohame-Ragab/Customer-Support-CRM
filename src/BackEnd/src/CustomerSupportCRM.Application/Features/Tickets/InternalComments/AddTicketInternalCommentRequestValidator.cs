using FluentValidation;

namespace CustomerSupportCRM.Application.Features.Tickets.InternalComments;

public sealed class AddTicketInternalCommentRequestValidator : AbstractValidator<AddTicketInternalCommentRequest>
{
    public AddTicketInternalCommentRequestValidator()
    {
        RuleFor(x => x.Body).NotEmpty().MaximumLength(4000);
    }
}
