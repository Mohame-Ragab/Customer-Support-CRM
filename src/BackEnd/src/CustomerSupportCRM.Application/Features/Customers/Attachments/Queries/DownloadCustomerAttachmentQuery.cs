using MediatR;

namespace CustomerSupportCRM.Application.Features.Customers.Attachments.Queries;

public sealed record DownloadCustomerAttachmentQuery(Guid CustomerId, Guid AttachmentId)
    : IRequest<DownloadCustomerAttachmentResult>;

public sealed record DownloadCustomerAttachmentResult(Stream Content, string FileName, string ContentType);
