using CustomerSupportCRM.Application.Common.Exceptions;
using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Features.Roles;
using CustomerSupportCRM.Application.Features.Roles.Dtos;
using CustomerSupportCRM.Domain.Constants;
using CustomerSupportCRM.Domain.Exceptions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CustomerSupportCRM.Infrastructure.Identity.Services;

/// <summary>
/// Minimal admin surface for listing roles and assigning exactly one role to a
/// user (security-admin/manage-roles). Does not create/rename/delete roles -
/// see the story's "not in scope" note.
/// </summary>
public sealed class RoleAdminService : IRoleAdminService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuditLogService _auditLogService;

    public RoleAdminService(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        IAuditLogService auditLogService)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _auditLogService = auditLogService;
    }

    public async Task<IReadOnlyList<RoleDto>> ListRolesAsync(CancellationToken ct)
    {
        return await _roleManager.Roles
            .AsNoTracking()
            .Select(r => new RoleDto(r.Id, r.Name!))
            .ToListAsync(ct);
    }

    public async Task AssignRoleAsync(AssignRoleRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            throw new NotFoundException(nameof(ApplicationUser), request.UserId);
        }

        if (!await _roleManager.RoleExistsAsync(request.RoleName))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(request.RoleName), $"Role '{request.RoleName}' does not exist."),
            });
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Any(r => string.Equals(r, request.RoleName, StringComparison.OrdinalIgnoreCase)))
        {
            return; // idempotent: already assigned
        }

        if (currentRoles.Count > 0)
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                throw ToValidationException(removeResult);
            }
        }

        var addResult = await _userManager.AddToRoleAsync(user, request.RoleName);
        if (!addResult.Succeeded)
        {
            throw ToValidationException(addResult);
        }

        // Demonstration invocation of the audit writer contract shipped by
        // security-admin/view-audit-logs - see that story's "Not in scope" note.
        await _auditLogService.WriteAsync(
            AuditActions.RoleAssigned,
            entityType: "User",
            entityId: user.Id.ToString(),
            summary: $"Assigned role '{request.RoleName}' to user {user.Email}.",
            metadata: new { request.UserId, request.RoleName },
            cancellationToken: ct);
    }

    private static ValidationException ToValidationException(IdentityResult result)
    {
        var failures = result.Errors.Select(e => new ValidationFailure(nameof(AssignRoleRequest.RoleName), e.Description));
        return new ValidationException(failures);
    }
}
