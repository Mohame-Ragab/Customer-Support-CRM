using FluentValidation;

namespace CustomerSupportCRM.Application.Features.KnowledgeBase.Search;

public sealed class SearchKnowledgeBaseQueryValidator : AbstractValidator<SearchKnowledgeBaseQuery>
{
    private static readonly string[] AllowedLanguages = ["en", "ar"];

    public SearchKnowledgeBaseQueryValidator()
    {
        RuleFor(x => x.Query)
            .NotEmpty().WithMessage("Search query is required.")
            .Must(q => string.IsNullOrEmpty(q) || q.Trim().Length >= 2)
            .WithMessage("Search query must be at least 2 characters.")
            .Must(q => string.IsNullOrEmpty(q) || q.Trim().Length <= 200)
            .WithMessage("Search query must be 200 characters or fewer.");

        RuleFor(x => x.Language)
            .Must(lang => string.IsNullOrEmpty(lang) || AllowedLanguages.Contains(lang.ToLowerInvariant()))
            .WithMessage("Language must be \"en\" or \"ar\".");

        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}
