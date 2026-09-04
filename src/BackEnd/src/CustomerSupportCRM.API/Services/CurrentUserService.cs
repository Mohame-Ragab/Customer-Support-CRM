using System.Security.Claims;
using CustomerSupportCRM.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CustomerSupportCRM.API.Services;

/// <summary>
/// HTTP-request-backed implementation of <see cref="ICurrentUserService"/>. Lives
/// in the API project (rather than Infrastructure) because it depends on
/// <see cref="IHttpContextAccessor"/>, which only a Web SDK project gets for free
/// from the ASP.NET Core shared framework. Returns "nobody" for background
/// operations/requests with no <see cref="HttpContext"/>, never throws.
/// </summary>
public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public Guid? UserId
    {
        get
        {
            var value = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    // Bug fix: JwtTokenGenerator emits the plain "name" claim (not the
    // ClaimTypes.Name long-form URI), and ASP.NET Core's default inbound JWT
    // claim-type map does not remap "name" the way it remaps "sub"
    // (-> NameIdentifier) or "role" (-> Role) - confirmed empirically: this
    // lookup returned null for every authenticated request, so
    // TicketHistoryWriter's ActorDisplayName and every ticket-internal-comment
    // author name were always null. Same fix already applied to Email below.
    public string? UserName => User?.FindFirstValue("name") ?? User?.FindFirstValue(ClaimTypes.Name) ?? User?.Identity?.Name;

    public string? Email => User?.FindFirstValue(ClaimTypes.Email) ?? User?.FindFirstValue("email");

    public IReadOnlyList<string> Roles =>
        User?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray() ?? [];

    public bool IsInRole(string role) => User?.IsInRole(role) ?? false;
}
