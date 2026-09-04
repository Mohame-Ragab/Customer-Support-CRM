namespace CustomerSupportCRM.Domain.Common;

/// <summary>
/// Base class for every domain entity. Centralizes identity and audit/soft-delete
/// concerns so persistence infrastructure can populate them consistently without
/// the Domain layer knowing anything about EF Core, HTTP, or Identity.
/// </summary>
/// <remarks>
/// <see cref="CreatedBy"/> and <see cref="UpdatedBy"/> store the acting user's
/// identifier as text (rather than a hard reference to an Identity type) so the
/// Domain layer never needs to know about ASP.NET Core Identity. They are
/// populated automatically by Infrastructure using <c>ICurrentUserService</c>,
/// and may be <c>null</c> for unauthenticated or background operations.
/// All timestamps are stored in UTC.
/// </remarks>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Soft-delete marker. Entities with <c>IsDeleted == true</c> are excluded
    /// from normal queries via an EF Core global query filter configured in
    /// Infrastructure. Never removed physically through normal repository use.
    /// </summary>
    public bool IsDeleted { get; set; }
}
