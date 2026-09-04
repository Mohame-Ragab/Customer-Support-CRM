using CustomerSupportCRM.Application.Features.RolePermissions.Dtos;

namespace CustomerSupportCRM.Application.Features.RolePermissions;

/// <summary>
/// Admin read/write access to the permission catalog and role-permission
/// assignments (security-admin/manage-role-permissions). Kept as an
/// abstraction in Application so MediatR handlers never reference
/// <c>RoleManager&lt;ApplicationRole&gt;</c> directly.
/// </summary>
public interface IRolePermissionAdminService
{
    IReadOnlyCollection<string> GetCatalog();

    Task<RolePermissionsDto> GetRolePermissionsAsync(Guid roleId, CancellationToken ct);

    Task AssignPermissionAsync(Guid roleId, string permissionName, CancellationToken ct);

    Task RemovePermissionAsync(Guid roleId, string permissionName, CancellationToken ct);
}
