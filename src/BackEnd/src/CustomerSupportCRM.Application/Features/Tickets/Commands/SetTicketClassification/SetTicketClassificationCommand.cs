using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using CustomerSupportCRM.Domain.Enums;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.Commands.SetTicketClassification;

public sealed record SetTicketClassificationCommand(
    Guid TicketId,
    Guid? CategoryId,
    TicketPriority? Priority) : IRequest<TicketDto>;
