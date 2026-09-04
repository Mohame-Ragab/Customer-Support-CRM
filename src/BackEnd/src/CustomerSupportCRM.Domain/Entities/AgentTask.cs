using CustomerSupportCRM.Domain.Common;

namespace CustomerSupportCRM.Domain.Entities;

/// <summary>
/// A personal follow-up reminder owned by exactly one agent (Identity user).
/// Never shared; every read/write is scoped to <see cref="OwnerUserId"/>
/// matching <c>ICurrentUserService.UserId</c> (F04 agent-dashboard/manage-tasks-and-reminders).
/// </summary>
public class AgentTask : BaseEntity
{
    public Guid OwnerUserId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime DueAt { get; set; } // UTC
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; } // UTC

    // Reserved for a future "link task to ticket" flow; no FK constraint (see
    // AgentTaskConfiguration) so this remains valid even if the referenced
    // ticket is later deleted.
    public Guid? TicketId { get; set; }
}
