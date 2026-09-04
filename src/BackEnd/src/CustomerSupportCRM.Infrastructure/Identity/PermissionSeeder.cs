using CustomerSupportCRM.Domain.Constants;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CustomerSupportCRM.Infrastructure.Identity;

/// <summary>
/// Ensures the <c>Admin</c> role always holds every permission in
/// <see cref="Permissions.All"/> (security-admin/manage-role-permissions).
/// Idempotent - checks existence before inserting. Does not grant permissions
/// to any other role by default; that is an admin decision made through
/// <c>RolePermissionsController</c>. Mirrors <see cref="RoleSeeder"/>'s
/// constructor-injection-free, try/catch startup pattern.
/// </summary>
public static class PermissionSeeder
{
    public static async Task SeedAsync(
        RoleManager<ApplicationRole> roleManager, ApplicationDbContext context, ILogger logger)
    {
        try
        {
            var adminRole = await roleManager.FindByNameAsync(Roles.Admin);
            if (adminRole == null)
            {
                return; // RoleSeeder has not run yet / DB not reachable; nothing to seed against.
            }

            var existingPermissions = await context.RolePermissions
                .Where(rp => rp.RoleId == adminRole.Id)
                .Select(rp => rp.PermissionName)
                .ToListAsync();

            var missing = Permissions.All.Except(existingPermissions).ToList();
            if (missing.Count == 0)
            {
                return;
            }

            foreach (var permission in missing)
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = adminRole.Id,
                    PermissionName = permission,
                });
            }

            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Startup must not fail just because the database isn't reachable yet.
            logger.LogWarning(ex, "Skipped permission seeding: the database was not reachable.");
        }
    }
}
