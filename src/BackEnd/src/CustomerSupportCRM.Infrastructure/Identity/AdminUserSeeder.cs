using CustomerSupportCRM.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CustomerSupportCRM.Infrastructure.Identity;

/// <summary>
/// Ensures a known Admin account exists so a freshly initialized database has
/// a way in (auth/user-identity gap fix - there was previously no seeded
/// staff user at all: RoleSeeder only creates roles, UserManagementService
/// only creates users through an already-authenticated Admin session, and
/// self-registration always lands in the Customer role). Reads credentials
/// from configuration - "AdminSeed:Email" / "AdminSeed:Password"
/// (environment variables: ADMINSEED__EMAIL / ADMINSEED__PASSWORD) - never
/// hard-coded. If either is missing, seeding is skipped entirely (documented,
/// not silently defaulted to a guessable password).
///
/// Idempotent: checks for an existing user by email first. An existing Admin
/// account is never overwritten (password, FullName, etc. are left exactly
/// as they are) - only its role membership is topped up if somehow missing.
/// Mirrors RoleSeeder/PermissionSeeder's try/catch startup pattern so a
/// database that isn't reachable yet cannot fail application startup.
/// </summary>
public static class AdminUserSeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager, IConfiguration configuration, ILogger logger)
    {
        var email = configuration["AdminSeed:Email"];
        var password = configuration["AdminSeed:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogInformation(
                "Skipped Admin account seeding: AdminSeed:Email/AdminSeed:Password are not configured.");
            return;
        }

        try
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            var existing = await userManager.FindByEmailAsync(normalizedEmail);

            if (existing is not null)
            {
                // Already seeded (or a real account happens to use this
                // email) - never touch its password or profile. Only ensure
                // it actually holds the Admin role, in case the role was
                // seeded after this ran on a previous, older database state.
                if (!await userManager.IsInRoleAsync(existing, Roles.Admin))
                {
                    await userManager.AddToRoleAsync(existing, Roles.Admin);
                    logger.LogInformation("Added missing Admin role to existing seeded account {Email}.", normalizedEmail);
                }
                return;
            }

            var user = new ApplicationUser
            {
                UserName = normalizedEmail,
                Email = normalizedEmail,
                EmailConfirmed = true,
                FullName = "System Administrator",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                logger.LogWarning(
                    "Admin account seeding failed: {Errors}",
                    string.Join("; ", createResult.Errors.Select(e => e.Description)));
                return;
            }

            await userManager.AddToRoleAsync(user, Roles.Admin);
            logger.LogInformation("Seeded initial Admin account {Email}.", normalizedEmail);
        }
        catch (Exception ex)
        {
            // Startup must not fail just because the database isn't reachable yet.
            logger.LogWarning(ex, "Skipped Admin account seeding: the database was not reachable.");
        }
    }
}
