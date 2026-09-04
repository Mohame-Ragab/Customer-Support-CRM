using CustomerSupportCRM.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CustomerSupportCRM.Infrastructure.Identity;

/// <summary>
/// The authentication/identity account. Deliberately kept empty at this stage:
/// it represents "who can log in", not a CRM business concept. Future business
/// entities (e.g. Agent, Customer) must reference this user by <see cref="Id"/>
/// rather than inherit from it, keeping Domain independent of ASP.NET Core
/// Identity. See docs/architecture.md, "ApplicationUser vs Domain entities".
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>Display name for staff accounts created by an Administrator (security-admin/manage-users). Customer self-registration does not set this.</summary>
    public string? FullName { get; set; }

    /// <summary>Soft on/off switch for staff accounts, distinct from Identity's own lockout mechanism. Defaults to true so existing/seeded users remain enabled.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// UTC creation/last-modification timestamps for admin-facing user management
    /// (security-admin/manage-users). Set explicitly by <c>UserManagementService</c>
    /// rather than by <c>AuditableEntitySaveChangesInterceptor</c>, because
    /// <see cref="ApplicationUser"/> is an Identity type and does not inherit
    /// <see cref="Domain.Common.BaseEntity"/> (see docs/architecture.md,
    /// "ApplicationUser vs Domain entities").
    /// </summary>
    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
