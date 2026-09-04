using CustomerSupportCRM.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace CustomerSupportCRM.API.Authorization;

/// <summary>Requirement satisfied when the caller holds a role granted the named permission - see <see cref="PermissionPolicyProvider"/>.</summary>
public sealed class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission) => Permission = permission;
}

/// <summary>
/// Resolves the authenticated user's role claims and asks <see cref="IPermissionService"/>
/// (server-side, never the JWT itself) whether any of those roles grant the
/// required permission (security-admin/manage-role-permissions).
/// </summary>
public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionService _permissionService;

    public PermissionAuthorizationHandler(IPermissionService permissionService)
        => _permissionService = permissionService;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var roleNames = context.User.FindAll(System.Security.Claims.ClaimTypes.Role)
            .Select(c => c.Value)
            .Concat(context.User.FindAll("role").Select(c => c.Value))
            .Distinct()
            .ToList();

        if (roleNames.Count == 0)
        {
            return;
        }

        var permissions = await _permissionService.GetPermissionsForRolesAsync(roleNames);
        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
