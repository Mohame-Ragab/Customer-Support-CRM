using CustomerSupportCRM.Application.Features.Customers.Attachments.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Customers.Attachments.Queries;

public sealed class GetCustomerAttachmentsQueryHandler
    : IRequestHandler<GetCustomerAttachmentsQuery, IReadOnlyList<CustomerAttachmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCustomerAttachmentsQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<CustomerAttachmentDto>> Handle(
        GetCustomerAttachmentsQuery request, CancellationToken cancellationToken)
    {
        var customerExists = await _unitOfWork.Repository<Customer>()
            .ExistsAsync(c => c.Id == request.CustomerId, cancellationToken);

        if (!customerExists)
        {
            throw new NotFoundException(nameof(Customer), request.CustomerId);
        }

        var attachments = await _unitOfWork.Repository<CustomerAttachment>()
            .FindAsync(a => a.CustomerId == request.CustomerId, cancellationToken);

        return attachments
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new CustomerAttachmentDto(
                a.Id, a.CustomerId, a.FileName, a.ContentType, a.SizeBytes, a.CreatedBy, a.CreatedAt))
            .ToList();
    }
}
