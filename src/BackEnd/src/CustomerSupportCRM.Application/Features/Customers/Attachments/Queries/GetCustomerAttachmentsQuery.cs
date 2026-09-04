using CustomerSupportCRM.Application.Features.Customers.Attachments.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Customers.Attachments.Queries;

public sealed record GetCustomerAttachmentsQuery(Guid CustomerId) : IRequest<IReadOnlyList<CustomerAttachmentDto>>;
