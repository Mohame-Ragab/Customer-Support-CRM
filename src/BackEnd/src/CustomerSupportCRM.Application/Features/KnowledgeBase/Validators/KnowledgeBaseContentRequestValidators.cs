using CustomerSupportCRM.Application.Features.KnowledgeBase.Dtos;
using FluentValidation;

namespace CustomerSupportCRM.Application.Features.KnowledgeBase.Validators;

file static class Rules
{
    public static readonly string[] AllowedLanguages = ["en", "ar"];
}

public sealed class CreateKnowledgeBaseContentRequestValidator : AbstractValidator<CreateKnowledgeBaseContentRequest>
{
    public CreateKnowledgeBaseContentRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Body).NotEmpty().MaximumLength(100_000);
        RuleFor(x => x.Summary).MaximumLength(500);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Language)
            .Must(lang => string.IsNullOrEmpty(lang) || Rules.AllowedLanguages.Contains(lang.ToLowerInvariant()))
            .WithMessage("Language must be \"en\" or \"ar\".");
    }
}

public sealed class UpdateKnowledgeBaseContentRequestValidator : AbstractValidator<UpdateKnowledgeBaseContentRequest>
{
    public UpdateKnowledgeBaseContentRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Body).NotEmpty().MaximumLength(100_000);
        RuleFor(x => x.Summary).MaximumLength(500);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Language)
            .Must(lang => string.IsNullOrEmpty(lang) || Rules.AllowedLanguages.Contains(lang.ToLowerInvariant()))
            .WithMessage("Language must be \"en\" or \"ar\".");
    }
}
