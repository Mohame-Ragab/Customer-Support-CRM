using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Enums;
using CustomerSupportCRM.Domain.Interfaces;

namespace CustomerSupportCRM.Application.Features.Tickets.History;

public sealed class TicketHistoryWriter : ITicketHistoryWriter
{
    private const int MaxValueLength = 512;
    private const int MaxNoteLength = 1000;

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public TicketHistoryWriter(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task AppendAsync(
        Guid ticketId,
        TicketHistoryEventType eventType,
        string? oldValue,
        string? newValue,
        string? note,
        CancellationToken cancellationToken)
    {
        var entry = new TicketHistoryEntry
        {
            TicketId = ticketId,
            EventType = eventType,
            OccurredAt = DateTime.UtcNow,
            ActorUserId = _currentUserService.UserId?.ToString(),
            ActorDisplayName = _currentUserService.UserName,
            OldValue = Truncate(oldValue, MaxValueLength),
            NewValue = Truncate(newValue, MaxValueLength),
            Note = Truncate(note, MaxNoteLength),
        };

        // Deliberately no SaveChangesAsync here - see interface doc comment;
        // the caller's own unit-of-work save persists this together with
        // their mutation.
        await _unitOfWork.Repository<TicketHistoryEntry>().AddAsync(entry, cancellationToken);
    }

    private static string? Truncate(string? value, int maxLength)
        => value != null && value.Length > maxLength ? value[..maxLength] : value;
}
