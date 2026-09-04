using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.InternalComments;

public sealed record ListTicketInternalCommentsQuery(Guid TicketId) : IRequest<IReadOnlyList<TicketInternalCommentDto>>;
