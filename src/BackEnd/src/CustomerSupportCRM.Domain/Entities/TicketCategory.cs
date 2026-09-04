using CustomerSupportCRM.Domain.Common;

namespace CustomerSupportCRM.Domain.Entities;

/// <summary>A fixed, seeded ticket category (tickets/set-ticket-category-and-priority). Admin CRUD over categories is out of scope here - see F10.</summary>
public class TicketCategory : BaseEntity
{
    public string Code { get; set; } = default!; // stable, e.g. "billing"
    public string NameEn { get; set; } = default!;
    public string NameAr { get; set; } = default!;
    public bool IsActive { get; set; } = true;
}
