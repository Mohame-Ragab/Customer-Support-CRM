using CustomerSupportCRM.Domain.Common;

namespace CustomerSupportCRM.Domain.Entities;

/// <summary>An append-only internal staff note attached to a customer (customers/manage-customer-notes). Edit/delete are out of scope for this story.</summary>
public class CustomerNote : BaseEntity
{
    public Guid CustomerId { get; set; }

    /// <summary>The ApplicationUser id of the staff member who authored the note.</summary>
    public string AuthorUserId { get; set; } = null!;

    public string Content { get; set; } = null!;
}
