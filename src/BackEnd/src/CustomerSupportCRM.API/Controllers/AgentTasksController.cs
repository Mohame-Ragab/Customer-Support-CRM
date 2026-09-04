using CustomerSupportCRM.Application.Features.Agents.Tasks.Commands;
using CustomerSupportCRM.Application.Features.Agents.Tasks.Dtos;
using CustomerSupportCRM.Application.Features.Agents.Tasks.Queries;
using CustomerSupportCRM.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupportCRM.API.Controllers;

/// <summary>Personal task/reminder list for the calling agent (F04 agent-dashboard/manage-tasks-and-reminders).</summary>
[ApiController]
[Route("api/agent-tasks")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Supervisor},{Roles.Manager},{Roles.Agent}")]
public sealed class AgentTasksController : BaseApiController
{
    private readonly IMediator _mediator;

    public AgentTasksController(IMediator mediator) => _mediator = mediator;

    /// <summary>GET /api/agent-tasks?includeCompleted=false - the caller's own tasks, ordered by due date.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AgentTaskDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AgentTaskDto>>> GetMine(
        [FromQuery] bool includeCompleted = false, CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetMyAgentTasksQuery(includeCompleted), ct));

    /// <summary>POST /api/agent-tasks - creates a task owned by the caller.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AgentTaskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AgentTaskDto>> Create([FromBody] CreateAgentTaskRequest request, CancellationToken ct)
    {
        var command = new CreateAgentTaskCommand(request.Description, request.DueAt, request.TicketId);
        var created = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetMine), created);
    }

    /// <summary>PUT /api/agent-tasks/{id} - updates the caller's own task (403 on ownership mismatch).</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AgentTaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AgentTaskDto>> Update(
        Guid id, [FromBody] UpdateAgentTaskRequest request, CancellationToken ct)
    {
        var command = new UpdateAgentTaskCommand(id, request.Description, request.DueAt, request.IsCompleted, request.TicketId);
        return Ok(await _mediator.Send(command, ct));
    }

    /// <summary>DELETE /api/agent-tasks/{id} - soft-deletes the caller's own task.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteAgentTaskCommand(id), ct);
        return NoContent();
    }
}
