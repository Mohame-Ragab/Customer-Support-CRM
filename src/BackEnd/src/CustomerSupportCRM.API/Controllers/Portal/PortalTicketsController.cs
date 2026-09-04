using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.CustomerPortal.Feedback;
using CustomerSupportCRM.Application.Features.CustomerPortal.Tickets;
using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using CustomerSupportCRM.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupportCRM.API.Controllers.Portal;

/// <summary>
/// Customer-facing ticket surface (F08 customer-portal): submit, list, view
/// own tickets, and leave feedback on a resolved/closed one. Strictly
/// self-scoped - every handler resolves identity from the JWT
/// (ICurrentUserService), never from a client-supplied id, and "not found"
/// is used uniformly for both "doesn't exist" and "isn't yours" so this
/// surface never confirms another customer's ticket exists.
/// </summary>
/// <remarks>
/// No separate "VerifiedCustomer" policy: <c>AuthenticationService</c> already
/// blocks login for Customer accounts with <c>EmailConfirmed == false</c>
/// (see Auth_EmailNotVerified), so any Customer JWT that reaches this
/// controller already belongs to a verified email - a second check here
/// would be redundant.
/// </remarks>
[ApiController]
[Route("api/customer-portal/tickets")]
[Authorize(Roles = Roles.Customer)]
public sealed class PortalTicketsController : BaseApiController
{
    private readonly IMediator _mediator;

    public PortalTicketsController(IMediator mediator) => _mediator = mediator;

    /// <summary>POST /api/customer-portal/tickets - submits a new ticket owned by the caller.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TicketDto>> Submit([FromBody] SubmitPortalTicketCommand command, CancellationToken ct)
    {
        var created = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>GET /api/customer-portal/tickets?page&amp;pageSize - the caller's own tickets, newest first.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TicketDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<TicketDto>>> GetMine(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetMyPortalTicketsQuery(page, pageSize), ct));

    /// <summary>GET /api/customer-portal/tickets/{id} - a single ticket, only if it belongs to the caller.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetMyPortalTicketByIdQuery(id), ct));

    /// <summary>POST /api/customer-portal/tickets/{id}/feedback - one rating (+optional comment) per resolved/closed ticket the caller owns.</summary>
    [HttpPost("{id:guid}/feedback")]
    [ProducesResponseType(typeof(SubmitFeedbackResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SubmitFeedbackResponse>> SubmitFeedback(
        Guid id, [FromBody] SubmitFeedbackRequest request, CancellationToken ct)
    {
        var command = new SubmitFeedbackCommand(id, request.Rating, request.Comment);
        var result = await _mediator.Send(command, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
}
