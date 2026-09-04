using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.Commands.AssignTicket;

public sealed record AssignTicketCommand(Guid TicketId, Guid AgentUserId) : IRequest<TicketDto>;
