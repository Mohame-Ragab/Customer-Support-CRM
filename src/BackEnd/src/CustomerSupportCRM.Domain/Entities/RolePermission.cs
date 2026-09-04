using CustomerSupportCRM.Domain.Common;

namespace CustomerSupportCRM.Domain.Entities;

/// <summary>
/// Grants a single permission string to a role (security-admin/manage-role-permissions).
/// No navigation property to <c>ApplicationRole</c> - Identity types live in
/// Infrastructure, and Domain must not reference them. The invariant
/// <c>(RoleId, PermissionName)</c> must be unique, enforced by a unique index in
/// <c>RolePermissionConfiguration</c>.
/// </summary>
public sealed class RolePermission : BaseEntity
{
    public Guid RoleId { get; set; }

    public string PermissionName { get; set; } = string.Empty;
}
