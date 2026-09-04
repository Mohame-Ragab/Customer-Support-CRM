using AutoMapper;
using CustomerSupportCRM.Application.Common.Exceptions;
using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using CustomerSupportCRM.Application.Features.Tickets.History;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Enums;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using FluentValidation.Results;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.Commands.SetTicketClassification;

public sealed class SetTicketClassificationCommandHandler : IRequestHandler<SetTicketClassificationCommand, TicketDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITicketHistoryWriter _historyWriter;
    private readonly IMapper _mapper;

    public SetTicketClassificationCommandHandler(
        IUnitOfWork unitOfWork, ITicketHistoryWriter historyWriter, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _historyWriter = historyWriter;
        _mapper = mapper;
    }

    public async Task<TicketDto> Handle(SetTicketClassificationCommand request, CancellationToken cancellationToken)
    {
        var ticketRepository = _unitOfWork.Repository<Ticket>();
        var ticket = await ticketRepository.GetByIdAsync(request.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new NotFoundException(nameof(Ticket), request.TicketId);
        }

        var oldCategoryId = ticket.CategoryId;
        var oldPriority = ticket.Priority;

        if (request.CategoryId.HasValue)
        {
            var category = await _unitOfWork.Repository<TicketCategory>()
                .GetByIdAsync(request.CategoryId.Value, cancellationToken);

            if (category == null || !category.IsActive)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(request.CategoryId), "The selected category is not valid."),
                });
            }

            ticket.CategoryId = category.Id;
            ticket.Category = category;
        }

        if (request.Priority.HasValue)
        {
            ticket.Priority = request.Priority.Value;
        }

        ticketRepository.Update(ticket);

        if (request.CategoryId.HasValue && oldCategoryId != ticket.CategoryId)
        {
            await _historyWriter.AppendAsync(
                ticket.Id, TicketHistoryEventType.CategoryChanged,
                oldValue: oldCategoryId?.ToString(), newValue: ticket.CategoryId?.ToString(),
                note: null, cancellationToken);
        }

        if (request.Priority.HasValue && oldPriority != ticket.Priority)
        {
            await _historyWriter.AppendAsync(
                ticket.Id, TicketHistoryEventType.PriorityChanged,
                oldValue: oldPriority?.ToString(), newValue: ticket.Priority?.ToString(),
                note: null, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await ticket.AttachCategoryIfNeededAsync(_unitOfWork, cancellationToken);
        return _mapper.Map<TicketDto>(ticket);
    }
}
