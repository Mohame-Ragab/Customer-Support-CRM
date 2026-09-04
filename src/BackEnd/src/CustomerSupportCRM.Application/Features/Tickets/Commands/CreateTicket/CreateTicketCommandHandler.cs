using AutoMapper;
using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using CustomerSupportCRM.Application.Features.Tickets.History;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Enums;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.Commands.CreateTicket;

public sealed class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, TicketDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITicketHistoryWriter _historyWriter;
    private readonly IMapper _mapper;

    public CreateTicketCommandHandler(IUnitOfWork unitOfWork, ITicketHistoryWriter historyWriter, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _historyWriter = historyWriter;
        _mapper = mapper;
    }

    public async Task<TicketDto> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var customerExists = await _unitOfWork.Repository<Customer>()
            .ExistsAsync(c => c.Id == request.CustomerId, cancellationToken);

        if (!customerExists)
        {
            throw new NotFoundException(nameof(Customer), request.CustomerId);
        }

        var ticket = new Ticket
        {
            Subject = request.Subject,
            Description = request.Description,
            CustomerId = request.CustomerId,
            Status = TicketStatus.New,
            Channel = request.Channel,
        };

        await _unitOfWork.Repository<Ticket>().AddAsync(ticket, cancellationToken);

        await _historyWriter.AppendAsync(
            ticket.Id,
            TicketHistoryEventType.Created,
            oldValue: null,
            newValue: $"{ticket.Subject}|{ticket.CustomerId}",
            note: ticket.Description,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TicketDto>(ticket);
    }
}
