using CustomerSupportCRM.Application.Features.Users.DTOs;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Users.Commands.UpdateUser;

/// <summary>Updates a staff account's mutable fields. Email and password are out of scope (see security-admin/manage-users, "not in scope").</summary>
public sealed record UpdateUserCommand(
    Guid Id,
    string FullName,
    string Role,
    bool IsActive) : IRequest<UserDto>;
