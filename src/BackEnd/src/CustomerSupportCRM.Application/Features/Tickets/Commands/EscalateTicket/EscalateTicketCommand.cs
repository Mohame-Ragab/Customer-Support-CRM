using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.Commands.EscalateTicket;

public sealed record EscalateTicketCommand(Guid TicketId, string? Reason) : IRequest<EscalateTicketResponse>;

public sealed record EscalateTicketResponse(Guid TicketId, bool IsEscalated, DateTime? EscalatedAt, bool AlreadyEscalated);
