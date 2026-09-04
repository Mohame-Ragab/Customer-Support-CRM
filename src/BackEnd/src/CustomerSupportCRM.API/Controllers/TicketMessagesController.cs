using CustomerSupportCRM.Application.Features.Tickets.Commands.SendEmailReply;
using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using CustomerSupportCRM.Application.Features.Tickets.Queries.GetTicketMessages;
using CustomerSupportCRM.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupportCRM.API.Controllers;

/// <summary>
/// Ticket email thread (F03 email-communication-channel): send an outbound
/// reply and view the message log. Staff-only; a separate anonymous
/// controller (<see cref="InboundEmailController"/>) handles inbound
/// ingestion since that has no user session at all.
/// </summary>
[ApiController]
[Route("api/tickets/{ticketId:guid}/messages")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Supervisor},{Roles.Manager},{Roles.Agent}")]
public sealed class TicketMessagesController : BaseApiController
{
    private readonly IMediator _mediator;

    public TicketMessagesController(IMediator mediator) => _mediator = mediator;

    /// <summary>GET /api/tickets/{ticketId}/messages - the ticket's email thread, oldest first.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TicketMessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<TicketMessageDto>>> GetList(Guid ticketId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetTicketMessagesQuery(ticketId), ct));

    /// <summary>POST /api/tickets/{ticketId}/messages/email - sends an agent reply by email.</summary>
    [HttpPost("email")]
    [ProducesResponseType(typeof(TicketMessageDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<TicketMessageDto>> SendEmail(
        Guid ticketId, [FromBody] SendEmailReplyRequest request, CancellationToken ct)
    {
        var command = new SendEmailReplyCommand(ticketId, request.Subject, request.BodyText, request.BodyHtml);
        var created = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetList), new { ticketId }, created);
    }
}
