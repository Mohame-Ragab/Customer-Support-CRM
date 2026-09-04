using CustomerSupportCRM.Application.Features.Users.DTOs;
using FluentValidation;
using DomainRoles = CustomerSupportCRM.Domain.Constants.Roles;

namespace CustomerSupportCRM.Application.Features.Users.Validators;

/// <summary>
/// Validates the PUT /api/users/{id} request body directly, since that is the
/// type <c>ValidationFilter</c> (API layer) actually binds and validates for
/// that action - the id comes from the route, validated separately by ASP.NET
/// Core's route-guid constraint. Mirrors <see cref="UpdateUserCommandValidator"/>'s
/// rules minus <c>Id</c>.
/// </summary>
public sealed class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    // Aliased (not "Roles") because the sibling Features.Roles namespace
    // (security-admin/manage-roles) would otherwise shadow Domain.Constants.Roles.
    private static readonly string[] StaffRoles =
    [
        DomainRoles.Admin, DomainRoles.Supervisor, DomainRoles.Manager, DomainRoles.Agent,
    ];

    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().Length(2, 100);

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(role => StaffRoles.Contains(role))
            .WithMessage($"Role must be one of: {string.Join(", ", StaffRoles)}.");
    }
}
