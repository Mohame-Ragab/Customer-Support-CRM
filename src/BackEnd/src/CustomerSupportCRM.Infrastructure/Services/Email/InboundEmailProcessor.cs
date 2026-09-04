using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CustomerSupportCRM.Infrastructure.Services.Email;

/// <summary>
/// Matches an inbound email to a ticket, primarily via the HMAC reply token
/// embedded in the "To" address (see SendEmailReplyCommandHandler), falling
/// back to In-Reply-To/References matching against previously stored
/// ProviderMessageId values. Lives in Infrastructure only because it uses EF
/// Core's IUnitOfWork.Repository<T>().Query() equivalent via the generic
/// repository (no EF/Identity type reference needed - it could live in
/// Application too, but is kept alongside SmtpTicketEmailSender for cohesion).
/// </summary>
public sealed partial class InboundEmailProcessor : IInboundEmailProcessor
{
    private static readonly Regex TokenPattern = MyRegex();

    private readonly IUnitOfWork _unitOfWork;
    private readonly EmailSettings _settings;
    private readonly ILogger<InboundEmailProcessor> _logger;

    public InboundEmailProcessor(
        IUnitOfWork unitOfWork, IOptions<EmailSettings> settings, ILogger<InboundEmailProcessor> logger)
    {
        _unitOfWork = unitOfWork;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<Guid?> ProcessAsync(InboundEmail email, CancellationToken cancellationToken)
    {
        var ticketId = await ResolveTicketIdAsync(email, cancellationToken);
        if (ticketId is null)
        {
            _logger.LogWarning("Inbound email from {From} could not be matched to a ticket", email.From);
            return null;
        }

        var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(ticketId.Value, cancellationToken);
        if (ticket is null)
        {
            return null;
        }

        var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(ticket.CustomerId, cancellationToken);
        string? failureReason = null;
        if (customer is null || !string.Equals(customer.Email, email.From, StringComparison.OrdinalIgnoreCase))
        {
            failureReason = "sender-mismatch";
            _logger.LogWarning(
                "Inbound email sender {From} does not match ticket {TicketId} customer email", email.From, ticketId);
        }

        var message = TicketMessage.CreateInbound(
            ticket.Id,
            fromAddress: email.From,
            toAddress: email.To,
            subject: email.Subject,
            bodyText: email.BodyText,
            bodyHtml: email.BodyHtml,
            providerMessageId: email.MessageId,
            inReplyTo: email.InReplyTo,
            failureReason: failureReason);

        // TODO(story-attachments): persist inbound attachments (discarded in v1).
        await _unitOfWork.Repository<TicketMessage>().AddAsync(message, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }

    private async Task<Guid?> ResolveTicketIdAsync(InboundEmail email, CancellationToken cancellationToken)
    {
        var match = TokenPattern.Match(email.To);
        if (match.Success)
        {
            var ticketHex = match.Groups[1].Value;
            var token = match.Groups[2].Value;

            if (Guid.TryParseExact(ticketHex, "N", out var ticketId) &&
                string.Equals(token, ComputeReplyToken(ticketId), StringComparison.OrdinalIgnoreCase))
            {
                return ticketId;
            }

            _logger.LogWarning("Inbound email reply token mismatch for {To}", email.To);
        }

        var candidates = new List<string>(email.References) { email.InReplyTo ?? string.Empty }
            .Where(id => !string.IsNullOrEmpty(id))
            .ToArray();

        if (candidates.Length == 0)
        {
            return null;
        }

        var matched = _unitOfWork.Repository<TicketMessage>().Query()
            .Where(m => m.ProviderMessageId != null && candidates.Contains(m.ProviderMessageId))
            .Select(m => (Guid?)m.TicketId)
            .FirstOrDefault();

        return matched;
    }

    private string ComputeReplyToken(Guid ticketId)
    {
        var secretBytes = Encoding.UTF8.GetBytes(_settings.Inbound.WebhookSecret);
        using var hmac = new HMACSHA256(secretBytes);
        var hash = hmac.ComputeHash(ticketId.ToByteArray());
        return Convert.ToHexString(hash)[..12].ToLowerInvariant();
    }

    [GeneratedRegex(@"^ticket\+([0-9a-fA-F]{32})\.([0-9a-f]{12})@", RegexOptions.IgnoreCase)]
    private static partial Regex MyRegex();
}
