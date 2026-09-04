using CustomerSupportCRM.Domain.Common;
using CustomerSupportCRM.Domain.Enums;

namespace CustomerSupportCRM.Domain.Entities;

/// <summary>
/// Aggregate root representing a customer support ticket (F02 — Ticket Management).
/// Fields are grouped by the story that introduced them:
/// - tickets/create-ticket: Subject, Description, Status, CustomerId.
/// - tickets/set-ticket-category-and-priority: CategoryId, Priority.
/// - tickets/assign-ticket: AssignedAgentId, AssignedAt.
/// - tickets/escalate-ticket: IsEscalated, EscalatedAt.
/// </summary>
public class Ticket : BaseEntity
{
    public string Subject { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.New;

    // FK to Customer (F01 - customers/create-customer, already shipped).
    public Guid CustomerId { get; set; }

    // tickets/set-ticket-category-and-priority. TicketCategory is a small,
    // seeded lookup table, so (unlike Customer) a navigation property is kept
    // for convenient projection in read handlers.
    public Guid? CategoryId { get; set; }
    public TicketCategory? Category { get; set; }
    public TicketPriority? Priority { get; set; }

    // tickets/assign-ticket. No navigation to ApplicationUser - Identity types
    // live in Infrastructure and Domain must stay independent of them.
    public Guid? AssignedAgentId { get; set; }
    public DateTime? AssignedAt { get; set; }

    // tickets/escalate-ticket.
    public bool IsEscalated { get; set; }
    public DateTime? EscalatedAt { get; set; }

    // F03 communication-channels: which intake channel created this ticket.
    // Defaults to Manual for staff-created (F02) tickets.
    public TicketChannel Channel { get; set; } = TicketChannel.Manual;
}
