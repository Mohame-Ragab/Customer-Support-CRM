namespace CustomerSupportCRM.Domain.Enums;

/// <summary>
/// Lifecycle status for a <see cref="Entities.Ticket"/> (tickets/create-ticket).
/// Initial value is <see cref="New"/>; transitions are governed by
/// tickets/change-ticket-status.
/// </summary>
public enum TicketStatus
{
    New = 0,
    InProgress = 1,
    Resolved = 2,
    Closed = 3,
}
