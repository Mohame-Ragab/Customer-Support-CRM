using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.History;

public sealed class GetTicketHistoryQueryHandler : IRequestHandler<GetTicketHistoryQuery, IReadOnlyList<TicketHistoryEntryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTicketHistoryQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<TicketHistoryEntryDto>> Handle(
        GetTicketHistoryQuery request, CancellationToken cancellationToken)
    {
        var ticketExists = await _unitOfWork.Repository<Ticket>()
            .ExistsAsync(t => t.Id == request.TicketId, cancellationToken);

        if (!ticketExists)
        {
            throw new NotFoundException(nameof(Ticket), request.TicketId);
        }

        var entries = await _unitOfWork.Repository<TicketHistoryEntry>()
            .FindAsync(e => e.TicketId == request.TicketId, cancellationToken);

        // Empty history is not an error (e.g. never happens in practice since
        // Created is always the first entry, but not treated as 404 either
        // way) - only an unknown ticket id is.
        return entries
            .OrderBy(e => e.OccurredAt)
            .ThenBy(e => e.Id)
            .Select(e => new TicketHistoryEntryDto(
                e.Id, e.TicketId, e.EventType, e.OccurredAt,
                e.ActorUserId, e.ActorDisplayName, e.OldValue, e.NewValue, e.Note))
            .ToList();
    }
}
