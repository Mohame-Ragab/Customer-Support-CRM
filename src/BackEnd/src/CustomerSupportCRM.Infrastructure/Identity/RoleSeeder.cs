using CustomerSupportCRM.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CustomerSupportCRM.Infrastructure.Identity;

/// <summary>
/// Ensures the well-known roles in <see cref="Roles"/> exist. Roles only - no
/// users, no passwords, no credentials are seeded here (per architecture scope:
/// no login/register/user-management feature exists yet to own that).
/// </summary>
public static class RoleSeeder
{
    public static async Task SeedAsync(RoleManager<ApplicationRole> roleManager, ILogger logger)
    {
        try
        {
            foreach (var roleName in new[] { Roles.Admin, Roles.Supervisor, Roles.Manager, Roles.Agent, Roles.Customer })
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
                }
            }
        }
        catch (Exception ex)
        {
            // Startup must not fail just because the database isn't reachable yet
            // (e.g. first deploy before migrations have run, or a dev machine with
            // no local SQL Server) - the health check already surfaces that state.
            logger.LogWarning(ex, "Skipped role seeding: the database was not reachable.");
        }
    }
}
