using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Features.Tickets.History;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Enums;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.Commands.EscalateTicket;

public sealed class EscalateTicketCommandHandler : IRequestHandler<EscalateTicketCommand, EscalateTicketResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITicketHistoryWriter _historyWriter;

    public EscalateTicketCommandHandler(
        IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ITicketHistoryWriter historyWriter)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _historyWriter = historyWriter;
    }

    public async Task<EscalateTicketResponse> Handle(EscalateTicketCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Ticket>();
        var ticket = await repository.GetByIdAsync(request.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new NotFoundException(nameof(Ticket), request.TicketId);
        }

        if (ticket.IsEscalated)
        {
            // Idempotent no-op: repeat escalation succeeds without a new audit row.
            return new EscalateTicketResponse(ticket.Id, true, ticket.EscalatedAt, AlreadyEscalated: true);
        }

        var now = DateTime.UtcNow;
        ticket.IsEscalated = true;
        ticket.EscalatedAt = now;
        repository.Update(ticket);

        await _unitOfWork.Repository<TicketEscalation>().AddAsync(
            new TicketEscalation
            {
                TicketId = ticket.Id,
                Reason = request.Reason,
                EscalatedAtUtc = now,
                EscalatedByUserId = _currentUserService.UserId?.ToString(),
                EscalatedByUserName = _currentUserService.UserName,
            },
            cancellationToken);

        await _historyWriter.AppendAsync(
            ticket.Id, TicketHistoryEventType.Escalated,
            oldValue: null, newValue: "Escalated", note: request.Reason, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new EscalateTicketResponse(ticket.Id, true, now, AlreadyEscalated: false);
    }
}
