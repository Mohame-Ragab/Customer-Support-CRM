using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using CustomerSupportCRM.Domain.Enums;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.Commands.CreateTicket;

/// <summary>
/// <paramref name="Channel"/> defaults to <see cref="TicketChannel.Manual"/> so
/// existing F02 callers (the staff "create ticket" form, JSON bodies without a
/// "channel" field) are unaffected; F03 intake channels (email/live-chat/web-form)
/// pass their own value when dispatching this command internally.
/// </summary>
public sealed record CreateTicketCommand(
    string Subject,
    string? Description,
    Guid CustomerId,
    TicketChannel Channel = TicketChannel.Manual) : IRequest<TicketDto>;
