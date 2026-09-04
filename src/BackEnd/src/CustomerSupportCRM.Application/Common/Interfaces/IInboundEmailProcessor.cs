namespace CustomerSupportCRM.Application.Common.Interfaces;

/// <summary>Ingests an inbound email (customer reply) and appends it to the matching ticket. Returns the ticket id, or null if no ticket could be matched.</summary>
public interface IInboundEmailProcessor
{
    Task<Guid?> ProcessAsync(InboundEmail email, CancellationToken cancellationToken);
}

public sealed record InboundEmail(
    string From,
    string To,
    string Subject,
    string BodyText,
    string? BodyHtml,
    string? MessageId,
    string? InReplyTo,
    IReadOnlyCollection<string> References);
