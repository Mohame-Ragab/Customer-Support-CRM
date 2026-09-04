using FluentValidation;

namespace CustomerSupportCRM.Application.Features.QuickReplies;

public sealed class CreateQuickReplyTemplateRequestValidator : AbstractValidator<CreateQuickReplyTemplateRequest>
{
    public CreateQuickReplyTemplateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Body).NotEmpty().MaximumLength(4000);
    }
}

public sealed class UpdateQuickReplyTemplateRequestValidator : AbstractValidator<UpdateQuickReplyTemplateRequest>
{
    public UpdateQuickReplyTemplateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Body).NotEmpty().MaximumLength(4000);
    }
}
