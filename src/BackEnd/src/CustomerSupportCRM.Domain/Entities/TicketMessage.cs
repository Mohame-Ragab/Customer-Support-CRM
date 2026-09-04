using CustomerSupportCRM.Domain.Common;
using CustomerSupportCRM.Domain.Enums;

namespace CustomerSupportCRM.Domain.Entities;

/// <summary>
/// A single inbound or outbound message on a ticket's email thread
/// (F03 communication-channels/email-communication-channel). Reuses
/// <see cref="TicketChannel"/> (Email only for now) instead of a
/// message-specific channel enum, since a ticket message's channel and its
/// owning ticket's intake channel are the same concept in v1.
/// </summary>
public sealed class TicketMessage : BaseEntity
{
    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;

    public TicketChannel Channel { get; set; } = TicketChannel.Email;
    public TicketMessageDirection Direction { get; set; }
    public EmailDeliveryStatus DeliveryStatus { get; set; }

    public string FromAddress { get; set; } = string.Empty;
    public string ToAddress { get; set; } = string.Empty;
    public string? Cc { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string BodyText { get; set; } = string.Empty;
    public string? BodyHtml { get; set; }

    public string? ProviderMessageId { get; set; }
    public string? InReplyToMessageId { get; set; }

    /// <summary>Opaque token echoed on the outbound Reply-To address; see InboundEmailProcessor.</summary>
    public string? ConversationToken { get; set; }

    public string? FailureReason { get; set; }

    /// <summary>Agent who sent this (outbound only). No FK to ApplicationUser - Identity lives in Infrastructure.</summary>
    public Guid? SentByUserId { get; set; }

    public static TicketMessage CreateOutbound(
        Guid ticketId, string fromAddress, string toAddress, string subject,
        string bodyText, string? bodyHtml, string? conversationToken, Guid? sentByUserId)
        => new()
        {
            TicketId = ticketId,
            Channel = TicketChannel.Email,
            Direction = TicketMessageDirection.Outbound,
            DeliveryStatus = EmailDeliveryStatus.Pending,
            FromAddress = fromAddress,
            ToAddress = toAddress,
            Subject = subject,
            BodyText = bodyText,
            BodyHtml = bodyHtml,
            ConversationToken = conversationToken,
            SentByUserId = sentByUserId,
        };

    public static TicketMessage CreateInbound(
        Guid ticketId, string fromAddress, string toAddress, string subject,
        string bodyText, string? bodyHtml, string? providerMessageId, string? inReplyTo, string? failureReason)
        => new()
        {
            TicketId = ticketId,
            Channel = TicketChannel.Email,
            Direction = TicketMessageDirection.Inbound,
            DeliveryStatus = EmailDeliveryStatus.Received,
            FromAddress = fromAddress,
            ToAddress = toAddress,
            Subject = subject,
            BodyText = bodyText,
            BodyHtml = bodyHtml,
            ProviderMessageId = providerMessageId,
            InReplyToMessageId = inReplyTo,
            FailureReason = failureReason,
        };
}
