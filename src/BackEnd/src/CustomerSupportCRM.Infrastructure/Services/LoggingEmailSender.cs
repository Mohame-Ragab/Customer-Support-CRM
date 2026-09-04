using CustomerSupportCRM.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace CustomerSupportCRM.Infrastructure.Services;

/// <summary>
/// Logging email sender. Logs email sends without actually sending.
/// TODO(FR-023): Replace with Email Communication Channel provider.
/// </summary>
public sealed class LoggingEmailSender : IEmailSender
{
    private readonly ILogger<LoggingEmailSender> _logger;

    public LoggingEmailSender(ILogger<LoggingEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default)
    {
        _logger.LogInformation("Email sent to {ToEmail}: {Subject}", toEmail, subject);
        return Task.CompletedTask;
    }
}
