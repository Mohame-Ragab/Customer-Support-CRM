using CustomerSupportCRM.Application.Features.Users.Commands.CreateUser;
using FluentValidation;
using DomainRoles = CustomerSupportCRM.Domain.Constants.Roles;

namespace CustomerSupportCRM.Application.Features.Users.Validators;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    // Staff roles assignable through this admin endpoint. DomainRoles.Customer is
    // deliberately excluded - customer accounts are created via
    // auth/customer-registration, not this staff-management surface.
    // Aliased (not "Roles") because the sibling Features.Roles namespace
    // (security-admin/manage-roles) would otherwise shadow Domain.Constants.Roles.
    private static readonly string[] StaffRoles =
    [
        DomainRoles.Admin, DomainRoles.Supervisor, DomainRoles.Manager, DomainRoles.Agent,
    ];

    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().Length(2, 100);

        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(role => StaffRoles.Contains(role))
            .WithMessage($"Role must be one of: {string.Join(", ", StaffRoles)}.");

        RuleFor(x => x.InitialPassword).NotEmpty().MinimumLength(8);
    }
}
