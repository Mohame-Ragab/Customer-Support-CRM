using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace CustomerSupportCRM.API.Authorization;

/// <summary>
/// Dynamically materializes an authorization policy for any permission string,
/// so call sites can write <c>[Authorize(Policy = Permissions.Tickets.Assign)]</c>
/// without registering every policy explicitly in Program.cs. Falls back to the
/// default provider (role/scheme-based policies, including plain
/// <c>[Authorize(Roles = ...)]</c>) for any policy name it doesn't recognize as
/// a permission string.
/// </summary>
public sealed class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackProvider;

    public PermissionPolicyProvider(Microsoft.Extensions.Options.IOptions<AuthorizationOptions> options)
        => _fallbackProvider = new DefaultAuthorizationPolicyProvider(options);

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallbackProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallbackProvider.GetFallbackPolicyAsync();

    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // Registered/explicit policies (if any are added later) win first.
        var existing = await _fallbackProvider.GetPolicyAsync(policyName);
        if (existing != null)
        {
            return existing;
        }

        // Any other policy name is treated as a permission string and gets a
        // requirement built on the fly - see Domain.Constants.Permissions.
        var policy = new AuthorizationPolicyBuilder();
        policy.AddRequirements(new PermissionRequirement(policyName));
        return policy.Build();
    }
}
