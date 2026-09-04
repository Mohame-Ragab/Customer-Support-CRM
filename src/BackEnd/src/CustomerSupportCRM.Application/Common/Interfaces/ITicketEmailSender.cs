namespace CustomerSupportCRM.Application.Common.Interfaces;

/// <summary>
/// Sends a ticket-thread email (F03 email-communication-channel). Deliberately
/// named <c>ITicketEmailSender</c>, not <c>IEmailSender</c> - that name is
/// already taken by the pre-existing, differently-shaped auth notification
/// port (<see cref="IEmailSender"/>, used for verification/reset emails);
/// reusing the name would collide, and the two ports serve different callers
/// (transactional auth email vs. a ticket message with headers/threading).
/// </summary>
public interface ITicketEmailSender
{
    Task<EmailSendResult> SendAsync(EmailMessage message, CancellationToken cancellationToken);
}

public sealed record EmailMessage(
    string To,
    string Subject,
    string BodyText,
    string? BodyHtml,
    string? InReplyTo,
    IReadOnlyDictionary<string, string>? Headers);

public sealed record EmailSendResult(bool Success, string? ProviderMessageId, string? Error);
