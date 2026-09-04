using CustomerSupportCRM.Domain.Common;

namespace CustomerSupportCRM.Domain.Entities;

/// <summary>
/// A canned-response template owned by a single agent (F04 agent-dashboard/use-quick-replies).
/// Personal scope only - no shared/team library in v1.
/// </summary>
public class QuickReplyTemplate : BaseEntity
{
    public Guid OwnerUserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
