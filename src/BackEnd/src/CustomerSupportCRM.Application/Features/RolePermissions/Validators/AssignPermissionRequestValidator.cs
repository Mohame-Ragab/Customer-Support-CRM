using CustomerSupportCRM.Application.Features.RolePermissions.Dtos;
using CustomerSupportCRM.Domain.Constants;
using FluentValidation;

namespace CustomerSupportCRM.Application.Features.RolePermissions.Validators;

/// <summary>
/// Validates the POST /api/admin/role-permissions/{roleId} request body
/// directly, since that is the type <c>ValidationFilter</c> (API layer)
/// actually binds and validates for that action.
/// </summary>
public sealed class AssignPermissionRequestValidator : AbstractValidator<AssignPermissionRequest>
{
    public AssignPermissionRequestValidator()
    {
        RuleFor(x => x.PermissionName)
            .NotEmpty()
            .MaximumLength(128)
            .Must(name => Permissions.All.Contains(name))
            .WithMessage("Unknown permission name.");
    }
}
