using FluentValidation;

namespace CustomerSupportCRM.Application.Features.Auth.Refresh;

public sealed class RefreshRequestValidator : AbstractValidator<RefreshRequest>
{
    public RefreshRequestValidator()
    {
        // RefreshToken is optional (see RefreshRequest.cs) - the normal case
        // is an empty body, with the token supplied via the HttpOnly cookie
        // instead. Only bound the length when a value is actually present.
        RuleFor(x => x.RefreshToken)
            .MaximumLength(512)
            .When(x => x.RefreshToken is not null);
    }
}
