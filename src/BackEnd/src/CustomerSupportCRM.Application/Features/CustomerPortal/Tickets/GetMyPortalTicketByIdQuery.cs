using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.CustomerPortal.Tickets;

public sealed record GetMyPortalTicketByIdQuery(Guid Id) : IRequest<TicketDto>;
