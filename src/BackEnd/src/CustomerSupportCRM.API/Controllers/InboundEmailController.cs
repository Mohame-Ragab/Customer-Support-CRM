using System.Security.Cryptography;
using System.Text;
using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.Tickets.Commands.IngestInboundEmail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CustomerSupportCRM.API.Controllers;

/// <summary>
/// Inbound email webhook (F03 email-communication-channel). Anonymous by
/// design (the mail relay/poller calling this has no user session) but gated
/// by a shared-secret header instead - never trust an unauthenticated request
/// without it.
/// </summary>
[ApiController]
[AllowAnonymous]
[Route("api/inbound/email")]
public sealed class InboundEmailController : BaseApiController
{
    private const string SecretHeaderName = "X-Inbound-Secret";

    private readonly IMediator _mediator;
    private readonly EmailSettings _settings;

    public InboundEmailController(IMediator mediator, IOptions<EmailSettings> settings)
    {
        _mediator = mediator;
        _settings = settings.Value;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Ingest([FromBody] InboundEmail email, CancellationToken ct)
    {
        if (!Request.Headers.TryGetValue(SecretHeaderName, out var provided) ||
            !IsSecretValid(provided.ToString()))
        {
            return Unauthorized();
        }

        var ticketId = await _mediator.Send(new IngestInboundEmailCommand(email), ct);

        return ticketId is null
            ? Accepted(new { ticketId = (Guid?)null })
            : Ok(new { ticketId });
    }

    private bool IsSecretValid(string provided)
    {
        var expected = _settings.Inbound.WebhookSecret;
        if (string.IsNullOrEmpty(expected) || string.IsNullOrEmpty(provided))
        {
            return false;
        }

        var expectedBytes = Encoding.UTF8.GetBytes(expected);
        var providedBytes = Encoding.UTF8.GetBytes(provided);

        return expectedBytes.Length == providedBytes.Length &&
            CryptographicOperations.FixedTimeEquals(expectedBytes, providedBytes);
    }
}
