using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Customers.Attachments.Queries;

public sealed class DownloadCustomerAttachmentQueryHandler
    : IRequestHandler<DownloadCustomerAttachmentQuery, DownloadCustomerAttachmentResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorage;

    public DownloadCustomerAttachmentQueryHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorage)
    {
        _unitOfWork = unitOfWork;
        _fileStorage = fileStorage;
    }

    public async Task<DownloadCustomerAttachmentResult> Handle(
        DownloadCustomerAttachmentQuery request, CancellationToken cancellationToken)
    {
        var attachment = await _unitOfWork.Repository<CustomerAttachment>()
            .GetByIdAsync(request.AttachmentId, cancellationToken);

        // Do not leak existence via a different status when the attachment
        // belongs to a different customer - always 404, same as unknown id.
        if (attachment == null || attachment.CustomerId != request.CustomerId)
        {
            throw new NotFoundException(nameof(CustomerAttachment), request.AttachmentId);
        }

        var stream = await _fileStorage.OpenReadAsync(attachment.StorageKey, cancellationToken);
        return new DownloadCustomerAttachmentResult(stream, attachment.FileName, attachment.ContentType);
    }
}
