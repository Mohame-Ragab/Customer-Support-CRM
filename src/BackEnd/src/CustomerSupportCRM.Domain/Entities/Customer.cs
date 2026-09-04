using CustomerSupportCRM.Domain.Common;

namespace CustomerSupportCRM.Domain.Entities;

/// <summary>
/// A CRM customer record maintained by staff (customers/create-customer).
/// Distinct from <c>ApplicationUser</c> (portal login identity), but a
/// Customer may optionally correspond to exactly one ApplicationUser -
/// <see cref="ApplicationUserId"/> is that link (1 ApplicationUser : 0..1
/// Customer; FK lives here since Customer is the optional side - a
/// staff-created Customer with no portal account has it null). Resolved and
/// backfilled by <c>ICustomerResolver</c> (see
/// Application/Features/Tickets/CustomerResolution) rather than set directly,
/// so every intake channel (registration, portal ticket submission, live
/// chat) shares one lookup/link/create implementation. No FK constraint to
/// AspNetUsers - Identity lives in Infrastructure and Domain must stay
/// independent of it (same convention as Ticket.AssignedAgentId).
/// </summary>
public class Customer : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? CompanyName { get; set; }
    public string? PreferredLanguage { get; set; } // "en" | "ar" | null
    public string? Notes { get; set; }

    /// <summary>The linked ApplicationUser's id, or null for a staff-managed customer with no portal account.</summary>
    public Guid? ApplicationUserId { get; set; }
}
