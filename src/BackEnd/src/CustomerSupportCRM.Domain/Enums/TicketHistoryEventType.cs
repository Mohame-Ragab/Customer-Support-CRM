namespace CustomerSupportCRM.Domain.Enums;

/// <summary>Kinds of lifecycle events recorded on a ticket's audit timeline (tickets/view-ticket-history).</summary>
public enum TicketHistoryEventType
{
    Created = 1,
    CategoryChanged = 2,
    PriorityChanged = 3,
    AssignmentChanged = 4,
    StatusChanged = 5,
    Escalated = 6,
}
