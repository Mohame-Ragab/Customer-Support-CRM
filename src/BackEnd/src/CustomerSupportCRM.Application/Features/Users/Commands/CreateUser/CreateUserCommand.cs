using CustomerSupportCRM.Application.Features.Users.DTOs;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Users.Commands.CreateUser;

/// <summary>Creates a staff account (Admin/Supervisor/Manager/Agent). Customer accounts are created via auth/customer-registration instead - see <see cref="Validators.CreateUserCommandValidator"/>.</summary>
public sealed record CreateUserCommand(
    string FullName,
    string Email,
    string Role,
    string InitialPassword) : IRequest<UserDto>;
