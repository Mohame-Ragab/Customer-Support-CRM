using CustomerSupportCRM.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CustomerSupportCRM.Infrastructure.Identity.Services;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ChangePasswordOutcome> ChangePasswordAsync(
        string userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken)
    {
        // Parse userId as Guid
        if (!Guid.TryParse(userId, out var userIdGuid))
        {
            return ChangePasswordOutcome.Failure(["user_not_found"]);
        }

        // Find user
        var user = await _userManager.FindByIdAsync(userIdGuid.ToString());
        if (user == null)
        {
            return ChangePasswordOutcome.Failure(["user_not_found"]);
        }

        // Change password
        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        if (!result.Succeeded)
        {
            var errors = result.Errors
                .Select(e => e.Code)
                .ToList();
            return ChangePasswordOutcome.Failure(errors);
        }

        // Update security stamp explicitly (ChangePasswordAsync already does this, but be explicit)
        await _userManager.UpdateSecurityStampAsync(user);

        return ChangePasswordOutcome.Success();
    }
}
