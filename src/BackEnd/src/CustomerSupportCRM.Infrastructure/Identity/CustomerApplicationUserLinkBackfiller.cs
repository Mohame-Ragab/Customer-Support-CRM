using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CustomerSupportCRM.Infrastructure.Identity;

/// <summary>
/// One-time-per-startup backfill for existing data created before
/// <c>Customer.ApplicationUserId</c> existed: links a Customer to an
/// ApplicationUser only when their emails match exactly and the match is
/// unambiguous (both tables already enforce a unique-per-active-row email
/// constraint, so a match is a 1:1 correspondence, never a guess). Rows that
/// don't have a clean 1:1 match are left untouched and logged for manual
/// review - never guessed. New Customer records don't need this; they're
/// linked directly by ICustomerResolver going forward. Idempotent: only
/// touches rows where ApplicationUserId is still null, so re-running does
/// nothing once the backfill has completed.
/// </summary>
public static class CustomerApplicationUserLinkBackfiller
{
    public static async Task RunAsync(
        ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger logger)
    {
        try
        {
            var unlinkedCustomers = await context.Customers
                .Where(c => c.ApplicationUserId == null)
                .ToListAsync();

            if (unlinkedCustomers.Count == 0)
            {
                return;
            }

            var linkedCount = 0;
            var ambiguous = new List<string>();

            foreach (var customer in unlinkedCustomers)
            {
                var user = await userManager.FindByEmailAsync(customer.Email);
                if (user is null)
                {
                    continue; // No portal account for this customer - nothing to link.
                }

                var alreadyLinkedToSomeoneElse = await context.Customers
                    .AnyAsync(c => c.ApplicationUserId == user.Id && c.Id != customer.Id);
                if (alreadyLinkedToSomeoneElse)
                {
                    // Should not happen given both tables' unique-email constraints,
                    // but never guess - report instead of silently picking one.
                    ambiguous.Add(customer.Email);
                    continue;
                }

                customer.ApplicationUserId = user.Id;
                linkedCount++;
            }

            if (linkedCount > 0)
            {
                await context.SaveChangesAsync();
                logger.LogInformation(
                    "Backfilled ApplicationUserId on {Count} existing Customer record(s) by email match.", linkedCount);
            }

            if (ambiguous.Count > 0)
            {
                logger.LogWarning(
                    "Could not safely backfill Customer.ApplicationUserId for {Count} email(s) - " +
                    "already linked to a different ApplicationUser; requires manual review: {Emails}",
                    ambiguous.Count, string.Join(", ", ambiguous));
            }
        }
        catch (Exception ex)
        {
            // Startup must not fail just because the database isn't reachable yet.
            logger.LogWarning(ex, "Skipped Customer/ApplicationUser link backfill: the database was not reachable.");
        }
    }
}
