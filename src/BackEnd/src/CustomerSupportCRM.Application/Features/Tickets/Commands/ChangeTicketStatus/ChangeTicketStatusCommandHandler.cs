using AutoMapper;
using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using CustomerSupportCRM.Application.Features.Tickets;
using CustomerSupportCRM.Application.Features.Tickets.History;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Enums;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.Commands.ChangeTicketStatus;

/// <summary>
/// Any transition between defined TicketStatus values is accepted - no state
/// machine (e.g. reopening a Closed ticket is allowed). A future story may
/// introduce transition rules.
/// </summary>
public sealed class ChangeTicketStatusCommandHandler : IRequestHandler<ChangeTicketStatusCommand, TicketDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITicketHistoryWriter _historyWriter;
    private readonly IMapper _mapper;

    public ChangeTicketStatusCommandHandler(
        IUnitOfWork unitOfWork, ITicketHistoryWriter historyWriter, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _historyWriter = historyWriter;
        _mapper = mapper;
    }

    public async Task<TicketDto> Handle(ChangeTicketStatusCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Ticket>();
        var ticket = await repository.GetByIdAsync(request.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new NotFoundException(nameof(Ticket), request.TicketId);
        }

        var oldStatus = ticket.Status;
        ticket.Status = request.Status;
        repository.Update(ticket);

        if (oldStatus != request.Status)
        {
            await _historyWriter.AppendAsync(
                ticket.Id, TicketHistoryEventType.StatusChanged,
                oldValue: oldStatus.ToString(), newValue: request.Status.ToString(),
                note: null, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await ticket.AttachCategoryIfNeededAsync(_unitOfWork, cancellationToken);
        return _mapper.Map<TicketDto>(ticket);
    }
}
