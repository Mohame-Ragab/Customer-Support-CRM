using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.Queries.GetTicketMessages;

public sealed record GetTicketMessagesQuery(Guid TicketId) : IRequest<IReadOnlyList<TicketMessageDto>>;
