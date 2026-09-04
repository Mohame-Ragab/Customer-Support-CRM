using AutoMapper;
using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.Queries.GetTicketMessages;

public sealed class GetTicketMessagesQueryHandler
    : IRequestHandler<GetTicketMessagesQuery, IReadOnlyList<TicketMessageDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTicketMessagesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TicketMessageDto>> Handle(
        GetTicketMessagesQuery request, CancellationToken cancellationToken)
    {
        var ticketExists = await _unitOfWork.Repository<Ticket>()
            .ExistsAsync(t => t.Id == request.TicketId, cancellationToken);
        if (!ticketExists)
        {
            throw new NotFoundException(nameof(Ticket), request.TicketId);
        }

        var messages = _unitOfWork.Repository<TicketMessage>().Query()
            .Where(m => m.TicketId == request.TicketId)
            .OrderBy(m => m.CreatedAt)
            .ToList();

        return _mapper.Map<List<TicketMessageDto>>(messages);
    }
}
