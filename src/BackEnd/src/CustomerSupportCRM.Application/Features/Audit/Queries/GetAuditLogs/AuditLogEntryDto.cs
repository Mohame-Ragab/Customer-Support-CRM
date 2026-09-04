namespace CustomerSupportCRM.Application.Features.Audit.Queries.GetAuditLogs;

public sealed record AuditLogEntryDto(
    Guid Id,
    DateTime TimestampUtc,
    string? PerformedByUserId,
    string? PerformedByUserName,
    string Action,
    string? EntityType,
    string? EntityId,
    string? Summary,
    string? MetadataJson);
