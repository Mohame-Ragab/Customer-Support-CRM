using AutoMapper;
using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.Queries.GetTicketById;

public sealed class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, TicketDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTicketByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TicketDto> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(request.Id, cancellationToken);
        if (ticket == null)
        {
            throw new NotFoundException(nameof(Ticket), request.Id);
        }

        // Application cannot call EF's Include() (Onion layering, same reason
        // as ToListAsync elsewhere); attach the category in-memory instead so
        // TicketMappingProfile's CategoryCode projection has something to read.
        if (ticket.CategoryId.HasValue)
        {
            ticket.Category = await _unitOfWork.Repository<TicketCategory>()
                .GetByIdAsync(ticket.CategoryId.Value, cancellationToken);
        }

        return _mapper.Map<TicketDto>(ticket);
    }
}
