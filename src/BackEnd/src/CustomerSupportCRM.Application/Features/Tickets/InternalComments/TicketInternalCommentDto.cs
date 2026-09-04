namespace CustomerSupportCRM.Application.Features.Tickets.InternalComments;

public sealed record TicketInternalCommentDto(
    Guid Id,
    Guid TicketId,
    string Body,
    string? AuthorId,
    string? AuthorDisplayName,
    DateTime CreatedAt);
