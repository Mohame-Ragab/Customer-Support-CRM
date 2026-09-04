namespace CustomerSupportCRM.Application.Common.Interfaces;

/// <summary>
/// Resolves the effective set of permissions granted to a set of role names
/// (security-admin/manage-role-permissions). Used server-side by the custom
/// authorization policy provider - permission claims are never trusted from
/// the JWT itself.
/// </summary>
public interface IPermissionService
{
    Task<IReadOnlyCollection<string>> GetPermissionsForRolesAsync(
        IEnumerable<string> roleNames, CancellationToken ct = default);
}
