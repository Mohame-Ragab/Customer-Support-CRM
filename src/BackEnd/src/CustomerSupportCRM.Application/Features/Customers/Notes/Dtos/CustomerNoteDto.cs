namespace CustomerSupportCRM.Application.Features.Customers.Notes.Dtos;

public sealed record CustomerNoteDto(
    Guid Id,
    Guid CustomerId,
    string AuthorUserId,
    string Content,
    DateTime CreatedAt);
