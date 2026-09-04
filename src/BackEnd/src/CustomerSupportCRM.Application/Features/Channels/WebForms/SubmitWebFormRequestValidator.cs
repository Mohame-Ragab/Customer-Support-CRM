using FluentValidation;

namespace CustomerSupportCRM.Application.Features.Channels.WebForms;

public sealed class SubmitWebFormRequestValidator : AbstractValidator<SubmitWebFormRequest>
{
    public SubmitWebFormRequestValidator()
    {
        RuleFor(x => x.SubmitterName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.SubmitterEmail).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Message).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.Category).MaximumLength(100);

        // Honeypot: a bot fills every field, including this hidden one. A human
        // never sees it, so any non-empty value is spam. The message is
        // deliberately generic - never reveal why the submission was rejected.
        RuleFor(x => x.Honeypot)
            .Must(string.IsNullOrEmpty)
            .WithMessage("This submission could not be processed.");
    }
}
