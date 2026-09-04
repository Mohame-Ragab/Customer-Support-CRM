using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.InternalComments;

public sealed record AddTicketInternalCommentCommand(Guid TicketId, string Body) : IRequest<TicketInternalCommentDto>;

/// <summary>Body-only shape bound via [FromBody] - see SendEmailReplyRequest for the same ValidationFilter rationale.</summary>
public sealed record AddTicketInternalCommentRequest(string Body);
