namespace CustomerSupportCRM.Application.Common.Interfaces;

/// <summary>
/// Abstraction over "who is making this request", used by Application use cases
/// and by Infrastructure's auditing interceptor. Deliberately exposes only plain
/// data (no <c>HttpContext</c>/<c>ClaimsPrincipal</c>/Identity types) so it can be
/// implemented from an HTTP request, a background job, or a test double alike.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>The authenticated user's id, or <c>null</c> for unauthenticated/background operations.</summary>
    Guid? UserId { get; }

    /// <summary>The authenticated user's display/user name, or <c>null</c> when unavailable.</summary>
    string? UserName { get; }

    /// <summary>
    /// The authenticated user's email, or <c>null</c> when unavailable. Added for
    /// F03 live-chat-communication-channel, which needs the authenticated
    /// Customer-role user's email to resolve/create their Customer CRM record
    /// (see Features/Tickets/CustomerResolution/ICustomerResolver.cs).
    /// </summary>
    string? Email { get; }

    bool IsAuthenticated { get; }

    IReadOnlyList<string> Roles { get; }

    bool IsInRole(string role);
}
