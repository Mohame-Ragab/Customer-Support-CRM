using CustomerSupportCRM.Domain.Common;

namespace CustomerSupportCRM.Domain.Entities;

/// <summary>
/// A staff-only internal note on a ticket (F04 agent-dashboard/team-collaboration-on-tickets).
/// Author is captured via <see cref="BaseEntity.CreatedBy"/> (set automatically by
/// AuditableEntitySaveChangesInterceptor) - never exposed on any customer-facing endpoint.
/// </summary>
public sealed class TicketInternalComment : BaseEntity
{
    public Guid TicketId { get; set; }
    public string Body { get; set; } = string.Empty;
}
