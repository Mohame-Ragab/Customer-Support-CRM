namespace CustomerSupportCRM.Domain.Exceptions;

/// <summary>
/// Thrown when an outbound ticket email could not be delivered by the SMTP
/// transport. The message row is still persisted (status Failed) before this
/// is thrown - see SendEmailReplyCommandHandler. Maps to HTTP 502.
/// </summary>
public sealed class EmailDeliveryException : DomainException
{
    public EmailDeliveryException(string message) : base(message)
    {
    }
}
