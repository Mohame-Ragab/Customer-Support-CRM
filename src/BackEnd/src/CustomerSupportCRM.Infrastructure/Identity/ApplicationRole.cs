using Microsoft.AspNetCore.Identity;

namespace CustomerSupportCRM.Infrastructure.Identity;

/// <summary>
/// The authentication/identity role. See <see cref="ApplicationUser"/> remarks -
/// role-based authorization is an Identity/Infrastructure concern, not a Domain one.
/// </summary>
public class ApplicationRole : IdentityRole<Guid>
{
}
