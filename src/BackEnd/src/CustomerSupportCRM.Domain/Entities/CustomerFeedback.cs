using CustomerSupportCRM.Domain.Common;

namespace CustomerSupportCRM.Domain.Entities;

/// <summary>
/// A customer's 1-5 satisfaction rating (+ optional comment) on one of their
/// own resolved/closed tickets (F08 customer-portal/submit-customer-feedback).
/// At most one per (TicketId, CustomerUserId) - enforced by a unique index.
/// </summary>
public sealed class CustomerFeedback : BaseEntity
{
    public Guid TicketId { get; set; }
    public Guid CustomerUserId { get; set; } // ApplicationUser.Id - no FK, Identity lives in Infrastructure.
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime SubmittedAt { get; set; }
}
