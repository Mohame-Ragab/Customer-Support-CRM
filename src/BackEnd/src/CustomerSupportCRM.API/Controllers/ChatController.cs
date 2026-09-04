using CustomerSupportCRM.Application.Features.Chat.Dtos;
using CustomerSupportCRM.Application.Features.Chat.Commands.StartChatSession;
using CustomerSupportCRM.Application.Features.Chat.Queries;
using CustomerSupportCRM.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupportCRM.API.Controllers;

/// <summary>REST surface for live chat (F03 live-chat-communication-channel); real-time messaging itself goes through <see cref="Hubs.ChatHub"/>.</summary>
[ApiController]
[Route("api/chat")]
[Authorize]
public sealed class ChatController : BaseApiController
{
    private readonly IMediator _mediator;

    public ChatController(IMediator mediator) => _mediator = mediator;

    /// <summary>POST /api/chat/sessions - the authenticated customer opens (or resumes) their chat session.</summary>
    [HttpPost("sessions")]
    [Authorize(Roles = Roles.Customer)]
    [ProducesResponseType(typeof(ChatSessionDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ChatSessionDto>> StartSession(CancellationToken ct)
    {
        var session = await _mediator.Send(new StartChatSessionCommand(), ct);
        return StatusCode(StatusCodes.Status201Created, session);
    }

    /// <summary>GET /api/chat/sessions/{id}/messages?take=&amp;beforeUtc= - paged message history.</summary>
    [HttpGet("sessions/{id:guid}/messages")]
    [ProducesResponseType(typeof(IReadOnlyList<ChatMessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ChatMessageDto>>> GetMessages(
        Guid id, [FromQuery] int take = 50, [FromQuery] DateTime? beforeUtc = null, CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetChatMessagesQuery(id, take, beforeUtc), ct));

    /// <summary>GET /api/chat/sessions/open - the agent lobby list.</summary>
    [HttpGet("sessions/open")]
    [Authorize(Roles = $"{Roles.Agent},{Roles.Supervisor},{Roles.Manager},{Roles.Admin}")]
    [ProducesResponseType(typeof(IReadOnlyList<ChatSessionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ChatSessionDto>>> GetOpenSessions(CancellationToken ct)
        => Ok(await _mediator.Send(new GetOpenChatSessionsQuery(), ct));
}
