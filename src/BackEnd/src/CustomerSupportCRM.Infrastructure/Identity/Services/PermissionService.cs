using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CustomerSupportCRM.Infrastructure.Identity.Services;

/// <summary>
/// Resolves the union of permissions granted to a set of role names, via
/// <c>RolePermissions</c> rows. Cached per individual role name (not per
/// role-name combination) for a short TTL, so a write to one role's
/// permissions can precisely evict just that role's cache entry -
/// see <see cref="PermissionCacheKeys"/>, used by both this reader and
/// <see cref="RolePermissionAdminService"/>'s write-side eviction.
/// </summary>
public sealed class PermissionService : IPermissionService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(60);

    private readonly ApplicationDbContext _context;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IMemoryCache _cache;

    public PermissionService(ApplicationDbContext context, RoleManager<ApplicationRole> roleManager, IMemoryCache cache)
    {
        _context = context;
        _roleManager = roleManager;
        _cache = cache;
    }

    public async Task<IReadOnlyCollection<string>> GetPermissionsForRolesAsync(
        IEnumerable<string> roleNames, CancellationToken ct = default)
    {
        var names = roleNames
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (names.Length == 0)
        {
            return Array.Empty<string>();
        }

        var union = new HashSet<string>(StringComparer.Ordinal);
        var uncachedRoleNames = new List<string>();

        foreach (var name in names)
        {
            if (_cache.TryGetValue(PermissionCacheKeys.ForRoleName(name), out IReadOnlyCollection<string>? cached) && cached != null)
            {
                union.UnionWith(cached);
            }
            else
            {
                uncachedRoleNames.Add(name);
            }
        }

        if (uncachedRoleNames.Count > 0)
        {
            var roles = await _roleManager.Roles
                .Where(r => uncachedRoleNames.Contains(r.Name!))
                .Select(r => new { r.Id, r.Name })
                .ToListAsync(ct);

            var roleIds = roles.Select(r => r.Id).ToList();
            var permissionRows = await _context.RolePermissions
                .Where(rp => roleIds.Contains(rp.RoleId))
                .ToListAsync(ct);

            foreach (var role in roles)
            {
                var rolePermissions = permissionRows
                    .Where(rp => rp.RoleId == role.Id)
                    .Select(rp => rp.PermissionName)
                    .Distinct()
                    .ToList();

                _cache.Set(PermissionCacheKeys.ForRoleName(role.Name!), (IReadOnlyCollection<string>)rolePermissions, CacheTtl);
                union.UnionWith(rolePermissions);
            }
        }

        return union;
    }
}

/// <summary>Cache-key shape shared between <see cref="PermissionService"/> (reads) and <see cref="RolePermissionAdminService"/> (write-side eviction).</summary>
internal static class PermissionCacheKeys
{
    public static string ForRoleName(string roleName) => $"permissions:role:{roleName.ToLowerInvariant()}";
}
