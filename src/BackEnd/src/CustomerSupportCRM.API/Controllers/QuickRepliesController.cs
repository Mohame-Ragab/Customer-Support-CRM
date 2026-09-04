using CustomerSupportCRM.Application.Features.QuickReplies;
using CustomerSupportCRM.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupportCRM.API.Controllers;

/// <summary>Personal canned-response templates for the calling agent (F04 agent-dashboard/use-quick-replies).</summary>
[ApiController]
[Route("api/quick-replies")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Supervisor},{Roles.Manager},{Roles.Agent}")]
public sealed class QuickRepliesController : BaseApiController
{
    private readonly IMediator _mediator;

    public QuickRepliesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<QuickReplyTemplateDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<QuickReplyTemplateDto>>> GetMine(
        [FromQuery] string? search, CancellationToken ct)
        => Ok(await _mediator.Send(new GetMyQuickReplyTemplatesQuery(search), ct));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(QuickReplyTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuickReplyTemplateDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetQuickReplyTemplateByIdQuery(id), ct));

    [HttpPost]
    [ProducesResponseType(typeof(QuickReplyTemplateDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<QuickReplyTemplateDto>> Create(
        [FromBody] CreateQuickReplyTemplateRequest request, CancellationToken ct)
    {
        var command = new CreateQuickReplyTemplateCommand(request.Name, request.Body);
        var created = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(QuickReplyTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<QuickReplyTemplateDto>> Update(
        Guid id, [FromBody] UpdateQuickReplyTemplateRequest request, CancellationToken ct)
    {
        var command = new UpdateQuickReplyTemplateCommand(id, request.Name, request.Body);
        return Ok(await _mediator.Send(command, ct));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteQuickReplyTemplateCommand(id), ct);
        return NoContent();
    }
}
