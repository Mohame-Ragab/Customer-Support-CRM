using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.Queries.GetTicketById;

public sealed record GetTicketByIdQuery(Guid Id) : IRequest<TicketDto>;
