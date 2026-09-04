namespace CustomerSupportCRM.Application.Features.Customers.Attachments.Dtos;

public sealed record CustomerAttachmentDto(
    Guid Id,
    Guid CustomerId,
    string FileName,
    string ContentType,
    long SizeBytes,
    string? UploadedBy,   // maps from CreatedBy
    DateTime UploadedAt); // maps from CreatedAt
