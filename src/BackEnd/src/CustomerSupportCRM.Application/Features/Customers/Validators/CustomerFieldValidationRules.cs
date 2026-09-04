using FluentValidation;

namespace CustomerSupportCRM.Application.Features.Customers.Validators;

/// <summary>
/// Field-level validation rules shared by CreateCustomerCommandValidator and
/// UpdateCustomerRequestValidator, so the two stay identical without
/// duplicating rule text (customers/create-customer, customers/update-customer).
/// </summary>
public static class CustomerFieldValidationRules
{
    public static void ApplyCustomerFieldRules<T>(this AbstractValidator<T> validator)
        where T : Customers.IHasCustomerContactFields
    {
        validator.RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        validator.RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        validator.RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);

        validator.RuleFor(x => x.PhoneNumber)
            .MaximumLength(32)
            .Matches(@"^[+0-9 ()\-]{5,32}$")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        validator.RuleFor(x => x.CompanyName).MaximumLength(200);

        validator.RuleFor(x => x.PreferredLanguage)
            .Must(lang => lang == "en" || lang == "ar")
            .WithMessage("PreferredLanguage must be 'en' or 'ar'.")
            .When(x => !string.IsNullOrWhiteSpace(x.PreferredLanguage));
    }
}
