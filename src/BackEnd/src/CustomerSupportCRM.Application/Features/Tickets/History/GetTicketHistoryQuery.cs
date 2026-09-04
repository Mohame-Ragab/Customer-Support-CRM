using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.History;

public sealed record GetTicketHistoryQuery(Guid TicketId) : IRequest<IReadOnlyList<TicketHistoryEntryDto>>;
