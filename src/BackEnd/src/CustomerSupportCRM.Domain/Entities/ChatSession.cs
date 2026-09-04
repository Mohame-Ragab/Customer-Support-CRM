using CustomerSupportCRM.Domain.Common;

namespace CustomerSupportCRM.Domain.Entities;

/// <summary>
/// A live-chat conversation (F03 communication-channels/live-chat-communication-channel),
/// always bound to a <see cref="Ticket"/> created on the first customer message.
/// </summary>
public sealed class ChatSession : BaseEntity
{
    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;

    /// <summary>
    /// The customer's ApplicationUser id (Identity). No FK - Identity types live
    /// in Infrastructure and Domain must stay independent of them (same
    /// convention as <c>Ticket.AssignedAgentId</c>).
    /// </summary>
    public Guid CustomerUserId { get; set; }

    public Guid? AssignedAgentUserId { get; set; }

    public DateTime StartedAtUtc { get; set; }
    public DateTime? EndedAtUtc { get; set; }

    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
