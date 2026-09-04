using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.Users.Commands.CreateUser;
using CustomerSupportCRM.Application.Features.Users.Commands.UpdateUser;
using CustomerSupportCRM.Application.Features.Users.DTOs;
using CustomerSupportCRM.Application.Features.Users.Queries.ListUsers;

namespace CustomerSupportCRM.Application.Common.Interfaces;

/// <summary>
/// Admin staff-account management (security-admin/manage-users). Kept as an
/// abstraction in Application so MediatR handlers never reference
/// <c>UserManager&lt;ApplicationUser&gt;</c> directly (that Identity type lives
/// in Infrastructure) - matches the <c>ICurrentUserService</c> pattern.
/// </summary>
public interface IUserManagementService
{
    Task<UserDto> CreateAsync(CreateUserCommand command, CancellationToken ct);

    Task<UserDto> UpdateAsync(UpdateUserCommand command, CancellationToken ct);

    Task<UserDto> GetByIdAsync(Guid id, CancellationToken ct);

    Task<PagedResult<UserDto>> ListAsync(ListUsersQuery query, CancellationToken ct);
}
