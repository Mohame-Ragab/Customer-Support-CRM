using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.Customers.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Customers.Queries.GetCustomers;

public sealed record GetCustomersQuery(int Page = 1, int PageSize = 20) : IRequest<PagedResult<CustomerDto>>;
