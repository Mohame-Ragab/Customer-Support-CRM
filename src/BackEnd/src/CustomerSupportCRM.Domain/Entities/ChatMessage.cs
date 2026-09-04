using CustomerSupportCRM.Domain.Common;
using CustomerSupportCRM.Domain.Enums;

namespace CustomerSupportCRM.Domain.Entities;

public sealed class ChatMessage : BaseEntity
{
    public Guid ChatSessionId { get; set; }
    public ChatSession Session { get; set; } = null!;

    public Guid TicketId { get; set; }

    /// <summary>Null for a system-generated message (e.g. "no agent available").</summary>
    public Guid? SenderUserId { get; set; }

    public ChatMessageKind Kind { get; set; }
    public string Body { get; set; } = string.Empty;
    public DateTime SentAtUtc { get; set; }
}
