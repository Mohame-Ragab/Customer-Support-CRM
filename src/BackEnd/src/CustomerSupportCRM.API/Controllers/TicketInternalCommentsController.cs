using CustomerSupportCRM.Application.Features.Tickets.InternalComments;
using CustomerSupportCRM.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupportCRM.API.Controllers;

/// <summary>
/// Staff-only internal comments on a ticket (F04 agent-dashboard/team-collaboration-on-tickets).
/// Never exposed on any customer-facing endpoint.
/// </summary>
[ApiController]
[Route("api/tickets/{ticketId:guid}/internal-comments")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Supervisor},{Roles.Manager},{Roles.Agent}")]
public sealed class TicketInternalCommentsController : BaseApiController
{
    private readonly IMediator _mediator;

    public TicketInternalCommentsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TicketInternalCommentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<TicketInternalCommentDto>>> List(Guid ticketId, CancellationToken ct)
        => Ok(await _mediator.Send(new ListTicketInternalCommentsQuery(ticketId), ct));

    [HttpPost]
    [ProducesResponseType(typeof(TicketInternalCommentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketInternalCommentDto>> Add(
        Guid ticketId, [FromBody] AddTicketInternalCommentRequest request, CancellationToken ct)
    {
        var comment = await _mediator.Send(new AddTicketInternalCommentCommand(ticketId, request.Body), ct);
        return CreatedAtAction(nameof(List), new { ticketId }, comment);
    }
}
