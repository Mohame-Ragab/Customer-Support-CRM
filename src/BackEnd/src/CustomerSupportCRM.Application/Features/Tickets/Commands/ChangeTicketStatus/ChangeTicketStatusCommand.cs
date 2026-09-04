using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using CustomerSupportCRM.Domain.Enums;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.Commands.ChangeTicketStatus;

public sealed record ChangeTicketStatusCommand(Guid TicketId, TicketStatus Status) : IRequest<TicketDto>;
