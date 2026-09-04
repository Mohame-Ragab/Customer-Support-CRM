namespace CustomerSupportCRM.Application.Common.Models;

/// <summary>
/// Generic page of results for list endpoints. Shared across features so every
/// paginated query returns the same envelope shape to the frontend.
/// </summary>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount);
