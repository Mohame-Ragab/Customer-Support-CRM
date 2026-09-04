namespace CustomerSupportCRM.Application.Features.Customers.InteractionHistory;

public sealed class CustomerInteractionHistoryResult
{
    public required Guid CustomerId { get; init; }
    public required IReadOnlyList<CustomerInteractionDto> Items { get; init; }
    public required int TotalCount { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
}
