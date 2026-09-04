using CustomerSupportCRM.Application.Features.Customers.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Customers.Queries.GetCustomerById;

public sealed record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerDto>;
