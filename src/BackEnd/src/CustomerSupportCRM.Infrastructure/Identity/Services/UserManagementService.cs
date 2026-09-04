using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.Users.Commands.CreateUser;
using CustomerSupportCRM.Application.Features.Users.Commands.UpdateUser;
using CustomerSupportCRM.Application.Features.Users.DTOs;
using CustomerSupportCRM.Application.Features.Users.Queries.ListUsers;
using CustomerSupportCRM.Domain.Constants;
using CustomerSupportCRM.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CustomerSupportCRM.Infrastructure.Identity.Services;

/// <summary>
/// Admin staff-account management (security-admin/manage-users): create, list,
/// view, and update Admin/Supervisor/Manager/Agent accounts. Customer accounts
/// (auth/customer-registration) and self-service profile fields
/// (auth/view-and-update-user-profile, <see cref="Users.Services.IUserProfileService"/>)
/// are out of scope here.
/// </summary>
public sealed class UserManagementService : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ICurrentUserService _currentUserService;

    public UserManagementService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _currentUserService = currentUserService;
    }

    public async Task<UserDto> CreateAsync(CreateUserCommand command, CancellationToken ct)
    {
        var normalizedEmail = command.Email.Trim().ToLowerInvariant();

        var existing = await _userManager.FindByEmailAsync(normalizedEmail);
        if (existing != null)
        {
            throw new ConflictException("A user with this email already exists.");
        }

        if (!await _roleManager.RoleExistsAsync(command.Role))
        {
            throw new NotFoundException("Role", command.Role);
        }

        var now = DateTime.UtcNow;
        var user = new ApplicationUser
        {
            UserName = normalizedEmail,
            Email = normalizedEmail,
            FullName = command.FullName,
            IsActive = true,
            EmailConfirmed = true, // staff accounts are provisioned by an Administrator, not self-verified
            CreatedAt = now,
        };

        var createResult = await _userManager.CreateAsync(user, command.InitialPassword);
        if (!createResult.Succeeded)
        {
            throw ToValidationException(createResult, nameof(command.InitialPassword));
        }

        var roleResult = await _userManager.AddToRoleAsync(user, command.Role);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            throw ToValidationException(roleResult, nameof(command.Role));
        }

        return ToDto(user, command.Role);
    }

    public async Task<UserDto> UpdateAsync(UpdateUserCommand command, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(command.Id.ToString());
        if (user == null)
        {
            throw new NotFoundException(nameof(ApplicationUser), command.Id);
        }

        if (!await _roleManager.RoleExistsAsync(command.Role))
        {
            throw new NotFoundException("Role", command.Role);
        }

        var isSelf = _currentUserService.UserId == command.Id;
        var currentRoles = await _userManager.GetRolesAsync(user);
        var currentRole = currentRoles.FirstOrDefault();
        var demotesOrDeactivatesSelf = isSelf
            && (!command.IsActive || !string.Equals(command.Role, Roles.Admin, StringComparison.Ordinal));

        if (demotesOrDeactivatesSelf)
        {
            throw new Application.Common.Exceptions.ValidationException(
                new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(command.IsActive),
                        "Administrators cannot demote or deactivate themselves."),
                });
        }

        var wouldRemoveLastAdmin =
            string.Equals(currentRole, Roles.Admin, StringComparison.Ordinal)
            && (!string.Equals(command.Role, Roles.Admin, StringComparison.Ordinal) || !command.IsActive);

        if (wouldRemoveLastAdmin)
        {
            var admins = await _userManager.GetUsersInRoleAsync(Roles.Admin);
            var otherActiveAdmins = admins.Any(a => a.Id != user.Id && a.IsActive);
            if (!otherActiveAdmins)
            {
                throw new ConflictException("At least one active Administrator is required.");
            }
        }

        if (!string.Equals(currentRole, command.Role, StringComparison.Ordinal))
        {
            if (currentRoles.Count > 0)
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    throw ToValidationException(removeResult, nameof(command.Role));
                }
            }

            var addResult = await _userManager.AddToRoleAsync(user, command.Role);
            if (!addResult.Succeeded)
            {
                throw ToValidationException(addResult, nameof(command.Role));
            }
        }

        user.FullName = command.FullName;
        user.IsActive = command.IsActive;
        user.ModifiedAt = DateTime.UtcNow;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            throw ToValidationException(updateResult, nameof(command.FullName));
        }

        return ToDto(user, command.Role);
    }

    public async Task<UserDto> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            throw new NotFoundException(nameof(ApplicationUser), id);
        }

        var roles = await _userManager.GetRolesAsync(user);
        return ToDto(user, roles.FirstOrDefault() ?? string.Empty);
    }

    public async Task<PagedResult<UserDto>> ListAsync(ListUsersQuery query, CancellationToken ct)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var usersQuery = _userManager.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLowerInvariant();
            usersQuery = usersQuery.Where(u =>
                (u.FullName != null && u.FullName.ToLower().Contains(search))
                || (u.Email != null && u.Email.ToLower().Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(query.Role))
        {
            var roleEntity = await _roleManager.FindByNameAsync(query.Role);
            if (roleEntity == null)
            {
                return new PagedResult<UserDto>([], page, pageSize, 0);
            }

            var userIdsInRole = (await _userManager.GetUsersInRoleAsync(query.Role))
                .Select(u => u.Id)
                .ToHashSet();
            usersQuery = usersQuery.Where(u => userIdsInRole.Contains(u.Id));
        }

        var totalCount = await usersQuery.CountAsync(ct);

        var pagedUsers = await usersQuery
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = new List<UserDto>(pagedUsers.Count);
        foreach (var user in pagedUsers)
        {
            var roles = await _userManager.GetRolesAsync(user);
            items.Add(ToDto(user, roles.FirstOrDefault() ?? string.Empty));
        }

        return new PagedResult<UserDto>(items, page, pageSize, totalCount);
    }

    private static UserDto ToDto(ApplicationUser user, string role) => new(
        user.Id,
        user.FullName,
        user.Email ?? string.Empty,
        role,
        user.IsActive,
        user.CreatedAt,
        user.ModifiedAt);

    private static Application.Common.Exceptions.ValidationException ToValidationException(
        IdentityResult result, string propertyName)
    {
        var failures = result.Errors.Select(e =>
            new FluentValidation.Results.ValidationFailure(propertyName, e.Description));
        return new Application.Common.Exceptions.ValidationException(failures);
    }
}
