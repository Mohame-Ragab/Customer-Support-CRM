using FluentValidation;
using CustomerSupportCRM.Application.Features.Users.DTOs;

namespace CustomerSupportCRM.Application.Features.Users.Validators;

public sealed class UpdateUserProfileRequestValidator : AbstractValidator<UpdateUserProfileRequest>
{
    public UpdateUserProfileRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
            .MaximumLength(256);

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[0-9\s\-()]{6,20}$")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
    }
}
