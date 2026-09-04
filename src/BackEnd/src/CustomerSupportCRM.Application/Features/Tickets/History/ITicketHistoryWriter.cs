using CustomerSupportCRM.Domain.Enums;

namespace CustomerSupportCRM.Application.Features.Tickets.History;

/// <summary>
/// Writer contract for the ticket audit timeline (tickets/view-ticket-history).
/// Every F02 lifecycle command (create, classify, assign, change status,
/// escalate) calls this after making its own change, within the SAME
/// unit-of-work scope - this method does not call SaveChangesAsync itself,
/// so the caller's own save persists the mutation and the history entry
/// together atomically.
/// </summary>
public interface ITicketHistoryWriter
{
    /// <summary>
    /// Queues a history entry for <paramref name="ticketId"/>. <paramref name="oldValue"/>/
    /// <paramref name="newValue"/> are truncated to 512 chars and <paramref name="note"/>
    /// to 1000 chars (matching the column limits) if the caller passes longer text.
    /// </summary>
    Task AppendAsync(
        Guid ticketId,
        TicketHistoryEventType eventType,
        string? oldValue,
        string? newValue,
        string? note,
        CancellationToken cancellationToken);
}
