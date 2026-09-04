using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.Tickets.Commands.AssignTicket;
using CustomerSupportCRM.Application.Features.Tickets.Commands.ChangeTicketStatus;
using CustomerSupportCRM.Application.Features.Tickets.Commands.CreateTicket;
using CustomerSupportCRM.Application.Features.Tickets.Commands.EscalateTicket;
using CustomerSupportCRM.Application.Features.Tickets.Commands.SetTicketClassification;
using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using CustomerSupportCRM.Application.Features.Tickets.History;
using CustomerSupportCRM.Application.Features.Tickets.Queries.GetTicketById;
using CustomerSupportCRM.Application.Features.Tickets.Queries.GetTicketCategories;
using CustomerSupportCRM.Application.Features.Tickets.Queries.GetTickets;
using CustomerSupportCRM.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupportCRM.API.Controllers;

/// <summary>
/// Staff-facing ticket lifecycle (F02 — Ticket Management): create, retrieve,
/// list, classify, assign, change status, escalate, and view history. All
/// endpoints are staff-only (Admin/Supervisor/Manager/Agent); assignment is
/// further restricted to Admin/Supervisor since agents cannot self-assign.
/// </summary>
[ApiController]
[Route("api/tickets")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Supervisor},{Roles.Manager},{Roles.Agent}")]
public sealed class TicketsController : BaseApiController
{
    private readonly IMediator _mediator;

    public TicketsController(IMediator mediator) => _mediator = mediator;

    /// <summary>POST /api/tickets - creates a ticket for an existing customer.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketDto>> Create([FromBody] CreateTicketCommand command, CancellationToken ct)
    {
        var created = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>GET /api/tickets/{id} - a single ticket.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetTicketByIdQuery(id), ct));

    /// <summary>
    /// GET /api/tickets?page&amp;pageSize&amp;assignedToMe - a paged list of tickets, newest first.
    /// assignedToMe=true (F04 agent-dashboard/view-assigned-tickets) restricts the list to
    /// tickets assigned to the calling agent.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TicketDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<TicketDto>>> GetList(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] bool assignedToMe = false,
        CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetTicketsQuery(page, pageSize, assignedToMe), ct));

    /// <summary>PATCH /api/tickets/{id}/classification - sets category and/or priority.</summary>
    [HttpPatch("{id:guid}/classification")]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketDto>> SetClassification(
        Guid id, [FromBody] SetTicketClassificationRequest request, CancellationToken ct)
    {
        var command = new SetTicketClassificationCommand(id, request.CategoryId, request.Priority);
        return Ok(await _mediator.Send(command, ct));
    }

    /// <summary>POST /api/tickets/{id}/assign - assigns the ticket to a user holding the Agent role. Admin/Supervisor only.</summary>
    [Authorize(Roles = $"{Roles.Admin},{Roles.Supervisor}")]
    [HttpPost("{id:guid}/assign")]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketDto>> Assign(Guid id, [FromBody] AssignTicketRequest request, CancellationToken ct)
    {
        var command = new AssignTicketCommand(id, request.AgentUserId);
        return Ok(await _mediator.Send(command, ct));
    }

    /// <summary>PATCH /api/tickets/{id}/status - changes the ticket's lifecycle status. Any transition between defined values is accepted.</summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketDto>> ChangeStatus(
        Guid id, [FromBody] ChangeTicketStatusRequest request, CancellationToken ct)
    {
        var command = new ChangeTicketStatusCommand(id, request.Status);
        return Ok(await _mediator.Send(command, ct));
    }

    /// <summary>POST /api/tickets/{id}/escalate - marks the ticket escalated. Idempotent: re-escalating returns AlreadyEscalated=true, no new audit row.</summary>
    [HttpPost("{id:guid}/escalate")]
    [ProducesResponseType(typeof(EscalateTicketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EscalateTicketResponse>> Escalate(
        Guid id, [FromBody] EscalateTicketRequest? request, CancellationToken ct)
    {
        var command = new EscalateTicketCommand(id, request?.Reason);
        return Ok(await _mediator.Send(command, ct));
    }

    /// <summary>GET /api/tickets/{id}/history - the ticket's chronological audit timeline.</summary>
    [HttpGet("{id:guid}/history")]
    [ProducesResponseType(typeof(IReadOnlyList<TicketHistoryEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<TicketHistoryEntryDto>>> GetHistory(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetTicketHistoryQuery(id), ct));
}

/// <summary>GET /api/ticket-categories - the active ticket category catalog, used to populate classification dropdowns.</summary>
[ApiController]
[Route("api/ticket-categories")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Supervisor},{Roles.Manager},{Roles.Agent}")]
public sealed class TicketCategoriesController : BaseApiController
{
    private readonly IMediator _mediator;

    public TicketCategoriesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TicketCategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TicketCategoryDto>>> GetAll(CancellationToken ct)
        => Ok(await _mediator.Send(new GetTicketCategoriesQuery(), ct));
}
