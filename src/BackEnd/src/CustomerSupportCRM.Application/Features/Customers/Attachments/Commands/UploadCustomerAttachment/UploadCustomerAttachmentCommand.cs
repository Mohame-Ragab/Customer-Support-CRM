using CustomerSupportCRM.Application.Features.Customers.Attachments.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Customers.Attachments.Commands.UploadCustomerAttachment;

public sealed record UploadCustomerAttachmentCommand(
    Guid CustomerId,
    Stream Content,
    string FileName,
    string ContentType,
    long SizeBytes) : IRequest<CustomerAttachmentDto>;
