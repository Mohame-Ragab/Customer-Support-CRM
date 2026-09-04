using CustomerSupportCRM.Domain.Common;

namespace CustomerSupportCRM.Domain.Entities;

/// <summary>Append-only audit row for a ticket escalation (tickets/escalate-ticket).</summary>
public class TicketEscalation : BaseEntity
{
    public Guid TicketId { get; set; }
    public string? Reason { get; set; }
    public DateTime EscalatedAtUtc { get; set; }
    public string? EscalatedByUserId { get; set; }
    public string? EscalatedByUserName { get; set; }
}
