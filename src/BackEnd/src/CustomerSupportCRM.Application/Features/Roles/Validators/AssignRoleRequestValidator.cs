using CustomerSupportCRM.Application.Features.Roles.Dtos;
using FluentValidation;

namespace CustomerSupportCRM.Application.Features.Roles.Validators;

public sealed class AssignRoleRequestValidator : AbstractValidator<AssignRoleRequest>
{
    public AssignRoleRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.RoleName).NotEmpty().MaximumLength(256);
    }
}
