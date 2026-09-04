using System.Net.Sockets;
using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Common.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CustomerSupportCRM.Infrastructure.Services.Email;

/// <summary>SMTP transport for ticket emails (F03 email-communication-channel), via MailKit.</summary>
public sealed class SmtpTicketEmailSender : ITicketEmailSender
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SmtpTicketEmailSender> _logger;

    public SmtpTicketEmailSender(IOptions<EmailSettings> settings, ILogger<SmtpTicketEmailSender> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<EmailSendResult> SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_settings.Smtp.Host))
        {
            // Deploying with an empty Email:Smtp:Host (e.g. before ops fills in
            // real credentials - see Migration/Rollback notes on EmailSettings)
            // is a delivery failure, not a server bug: fail fast with a normal
            // EmailSendResult instead of letting MailKit throw ArgumentException.
            _logger.LogWarning("Cannot send ticket email: Email:Smtp:Host is not configured");
            return new EmailSendResult(false, null, "SMTP is not configured.");
        }

        var mime = new MimeMessage();
        mime.From.Add(new MailboxAddress(_settings.FromDisplayName, _settings.FromAddress));
        mime.To.Add(MailboxAddress.Parse(message.To));
        mime.Subject = message.Subject;

        if (!string.IsNullOrEmpty(message.InReplyTo))
        {
            mime.InReplyTo = message.InReplyTo;
        }

        var builder = new BodyBuilder { TextBody = message.BodyText };
        if (!string.IsNullOrEmpty(message.BodyHtml))
        {
            builder.HtmlBody = message.BodyHtml;
        }
        mime.Body = builder.ToMessageBody();

        if (message.Headers is not null)
        {
            foreach (var (key, value) in message.Headers)
            {
                mime.Headers.Replace(key, value);
            }
        }

        try
        {
            using var client = new SmtpClient();
            var secureOption = _settings.Smtp.UseStartTls
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.None;

            await client.ConnectAsync(_settings.Smtp.Host, _settings.Smtp.Port, secureOption, cancellationToken);

            if (!string.IsNullOrEmpty(_settings.Smtp.User))
            {
                await client.AuthenticateAsync(_settings.Smtp.User, _settings.Smtp.Password, cancellationToken);
            }

            await client.SendAsync(mime, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            return new EmailSendResult(true, mime.MessageId, null);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex) when (ex is SmtpCommandException or SmtpProtocolException or SocketException or AuthenticationException)
        {
            _logger.LogWarning(ex, "Failed to send ticket email to {To}", message.To);
            return new EmailSendResult(false, null, ex.Message);
        }
    }
}
