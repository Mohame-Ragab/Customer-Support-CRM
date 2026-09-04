using MediatR;

namespace CustomerSupportCRM.Application.Features.Customers.InteractionHistory;

public sealed record GetCustomerInteractionHistoryQuery(
    Guid CustomerId,
    int Page = 1,
    int PageSize = 20) : IRequest<CustomerInteractionHistoryResult>;
