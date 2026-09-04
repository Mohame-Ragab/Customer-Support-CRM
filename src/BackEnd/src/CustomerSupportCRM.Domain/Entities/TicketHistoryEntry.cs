using CustomerSupportCRM.Domain.Common;
using CustomerSupportCRM.Domain.Enums;

namespace CustomerSupportCRM.Domain.Entities;

/// <summary>
/// Append-only unified audit-trail row for a ticket (tickets/view-ticket-history).
/// Written by <c>ITicketHistoryWriter</c>, called by every F02 lifecycle
/// command (create, classify, assign, change status, escalate) within the
/// same unit-of-work scope as their own mutation. Kept regardless of the
/// parent ticket's soft-delete state - the audit trail is never filtered.
/// </summary>
public class TicketHistoryEntry : BaseEntity
{
    public Guid TicketId { get; set; }
    public TicketHistoryEventType EventType { get; set; }
    public DateTime OccurredAt { get; set; } // business timestamp, set by the writer
    public string? ActorUserId { get; set; }
    public string? ActorDisplayName { get; set; } // denormalized snapshot
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Note { get; set; }
}
