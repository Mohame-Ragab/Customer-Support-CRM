using CustomerSupportCRM.Application.Features.Departments.DTOs;
using FluentValidation;

namespace CustomerSupportCRM.Application.Features.Departments.Validators;

public sealed class CreateDepartmentRequestValidator : AbstractValidator<CreateDepartmentRequest>
{
    public CreateDepartmentRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Code)
            .MaximumLength(20)
            .Matches(@"^[A-Za-z0-9]+$")
            .When(x => !string.IsNullOrWhiteSpace(x.Code));

        RuleFor(x => x.Description).MaximumLength(500);
    }
}
