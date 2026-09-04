using CustomerSupportCRM.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace CustomerSupportCRM.Infrastructure.Services;

/// <summary>
/// No-op email verification sender. Logs registration events without sending email.
/// TODO: Replaced by real implementation in auth/email-verification story.
/// </summary>
public sealed class NoopEmailVerificationSender : IEmailVerificationSender
{
    private readonly ILogger<NoopEmailVerificationSender> _logger;

    public NoopEmailVerificationSender(ILogger<NoopEmailVerificationSender> logger)
    {
        _logger = logger;
    }

    public Task QueueAsync(Guid userId, string email, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[stub] Email verification queued for user {UserId}", userId);
        return Task.CompletedTask;
    }
}
