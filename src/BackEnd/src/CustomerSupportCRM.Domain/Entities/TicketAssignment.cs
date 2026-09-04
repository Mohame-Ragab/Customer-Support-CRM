using CustomerSupportCRM.Domain.Common;

namespace CustomerSupportCRM.Domain.Entities;

/// <summary>Append-only audit row for a ticket assignment (tickets/assign-ticket). No FK to Identity users - referential integrity to ApplicationUser is enforced at the application layer.</summary>
public class TicketAssignment : BaseEntity
{
    public Guid TicketId { get; set; }
    public Guid AssignedAgentId { get; set; }
    public Guid AssignedByUserId { get; set; }
    public DateTime AssignedAtUtc { get; set; }
}
