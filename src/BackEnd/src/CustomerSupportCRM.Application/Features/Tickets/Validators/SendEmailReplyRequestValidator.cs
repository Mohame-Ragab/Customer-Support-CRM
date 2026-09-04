using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using FluentValidation;

namespace CustomerSupportCRM.Application.Features.Tickets.Validators;

public sealed class SendEmailReplyRequestValidator : AbstractValidator<SendEmailReplyRequest>
{
    public SendEmailReplyRequestValidator()
    {
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(200);
        RuleFor(x => x.BodyText).NotEmpty().MaximumLength(20000);
    }
}
