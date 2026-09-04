using CustomerSupportCRM.Domain.Enums;

namespace CustomerSupportCRM.Application.Features.Tickets.History;

public sealed record TicketHistoryEntryDto(
    Guid Id,
    Guid TicketId,
    TicketHistoryEventType EventType,
    DateTime OccurredAt,
    string? ActorUserId,
    string? ActorDisplayName,
    string? OldValue,
    string? NewValue,
    string? Note);
