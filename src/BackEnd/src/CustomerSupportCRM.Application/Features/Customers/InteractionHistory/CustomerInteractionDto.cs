namespace CustomerSupportCRM.Application.Features.Customers.InteractionHistory;

public sealed class CustomerInteractionDto
{
    public required Guid Id { get; init; }
    public required InteractionType Type { get; init; }
    public required DateTime OccurredAtUtc { get; init; } // chronological sort key
    public required string Title { get; init; }
    public string? Summary { get; init; }
    public string? Status { get; init; }              // e.g. ticket status name, nullable for future types
    public string? Channel { get; init; }             // nullable; populated by F03 later
    public Guid? SourceEntityId { get; init; }        // e.g. TicketId
}
