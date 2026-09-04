using CustomerSupportCRM.Application.Features.Users.Commands.UpdateUser;
using FluentValidation;
using DomainRoles = CustomerSupportCRM.Domain.Constants.Roles;

namespace CustomerSupportCRM.Application.Features.Users.Validators;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    // Aliased (not "Roles") because the sibling Features.Roles namespace
    // (security-admin/manage-roles) would otherwise shadow Domain.Constants.Roles.
    private static readonly string[] StaffRoles =
    [
        DomainRoles.Admin, DomainRoles.Supervisor, DomainRoles.Manager, DomainRoles.Agent,
    ];

    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.FullName).NotEmpty().Length(2, 100);

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(role => StaffRoles.Contains(role))
            .WithMessage($"Role must be one of: {string.Join(", ", StaffRoles)}.");
    }
}
