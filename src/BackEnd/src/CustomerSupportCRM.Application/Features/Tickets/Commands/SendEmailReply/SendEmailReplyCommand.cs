using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.Commands.SendEmailReply;

public sealed record SendEmailReplyCommand(
    Guid TicketId,
    string Subject,
    string BodyText,
    string? BodyHtml) : IRequest<TicketMessageDto>;
