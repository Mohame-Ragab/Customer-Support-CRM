namespace CustomerSupportCRM.Application.Common.Models;

/// <summary>Strongly typed binding of the "Email" configuration section (F03 email-communication-channel).</summary>
public sealed class EmailSettings
{
    public const string SectionName = "Email";

    public string FromAddress { get; init; } = string.Empty;
    public string FromDisplayName { get; init; } = "Support";

    /// <summary>Domain used to build the Reply-To / Message-ID headers, e.g. "reply.support.example.com".</summary>
    public string ReplyToDomain { get; init; } = string.Empty;

    public SmtpOptions Smtp { get; init; } = new();
    public InboundOptions Inbound { get; init; } = new();
}

public sealed class SmtpOptions
{
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; } = 587;
    public string User { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public bool UseStartTls { get; init; } = true;
}

public sealed class InboundOptions
{
    /// <summary>
    /// Shared secret required in the X-Inbound-Secret header, and used to HMAC the
    /// reply token embedded in outbound Reply-To addresses. Rotating this
    /// invalidates outstanding reply tokens; the In-Reply-To fallback in
    /// InboundEmailProcessor keeps threading working for messages sent before
    /// rotation.
    /// </summary>
    public string WebhookSecret { get; init; } = string.Empty;
}
