using CustomerSupportCRM.Application.Features.RolePermissions.Commands;
using CustomerSupportCRM.Domain.Constants;
using FluentValidation;

namespace CustomerSupportCRM.Application.Features.RolePermissions.Validators;

public sealed class AssignPermissionCommandValidator : AbstractValidator<AssignPermissionCommand>
{
    public AssignPermissionCommandValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty();

        RuleFor(x => x.PermissionName)
            .NotEmpty()
            .MaximumLength(128)
            .Must(name => Permissions.All.Contains(name))
            .WithMessage("Unknown permission name.");
    }
}
