namespace CustomerSupportCRM.Domain.Entities;

/// <summary>
/// An immutable, append-only record of a security-relevant administrative
/// action (security-admin/view-audit-logs). Deliberately does not inherit
/// <see cref="Common.BaseEntity"/>: audit entries are never updated or
/// soft-deleted, so they carry no <c>UpdatedBy</c>/<c>UpdatedAt</c>/<c>IsDeleted</c>.
/// Never contains passwords, tokens, or other secret material - see
/// <c>Common.Auditing.SensitiveFieldRedactor</c>, applied before <see cref="MetadataJson"/>
/// is populated.
/// </summary>
public sealed class AuditLogEntry
{
    public Guid Id { get; private set; }

    public DateTime TimestampUtc { get; private set; }

    /// <summary>Nullable: <c>null</c> for system/background actions with no authenticated actor.</summary>
    public string? PerformedByUserId { get; private set; }

    /// <summary>Captured at write time (not looked up later), so renaming/deleting the actor later doesn't rewrite history.</summary>
    public string? PerformedByUserName { get; private set; }

    /// <summary>See <see cref="Constants.AuditActions"/> for the closed set of action names.</summary>
    public string Action { get; private set; } = default!;

    public string? EntityType { get; private set; }

    public string? EntityId { get; private set; }

    public string? Summary { get; private set; }

    /// <summary>Redacted, serialized JSON payload. May be <c>null</c> when no metadata was supplied.</summary>
    public string? MetadataJson { get; private set; }

    private AuditLogEntry()
    {
    }

    public static AuditLogEntry Create(
        string action,
        string? performedByUserId,
        string? performedByUserName,
        string? entityType,
        string? entityId,
        string? summary,
        string? metadataJson)
    {
        return new AuditLogEntry
        {
            Id = Guid.NewGuid(),
            TimestampUtc = DateTime.UtcNow,
            PerformedByUserId = performedByUserId,
            PerformedByUserName = performedByUserName,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Summary = summary,
            MetadataJson = metadataJson,
        };
    }
}
