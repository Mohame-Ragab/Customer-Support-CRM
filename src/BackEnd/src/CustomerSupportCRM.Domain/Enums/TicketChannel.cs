namespace CustomerSupportCRM.Domain.Enums;

/// <summary>
/// How a ticket originated. Introduced by F03 (communication-channels) so the
/// three intake channel stories can tag tickets they create; existing F02
/// staff-created tickets default to <see cref="Manual"/>.
/// </summary>
public enum TicketChannel
{
    Manual = 1,
    Email = 2,
    LiveChat = 3,
    WebForm = 4,

    /// <summary>F08 customer-portal/submit-ticket-via-customer-portal. Stored as int - no migration needed.</summary>
    CustomerPortal = 5,
}
