using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.KnowledgeBase.Commands;
using CustomerSupportCRM.Application.Features.KnowledgeBase.Dtos;
using CustomerSupportCRM.Application.Features.KnowledgeBase.Queries;
using CustomerSupportCRM.Domain.Constants;
using CustomerSupportCRM.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupportCRM.API.Controllers;

/// <summary>
/// Staff authoring surface for Knowledge Base content (F06 knowledge-base/manage-knowledge-base-content):
/// FAQs, Articles, and Solution/Guides. Read-only public search lives in the
/// separate, anonymous <see cref="KnowledgeBaseController"/>.
/// </summary>
[ApiController]
[Route("api/knowledge-base/content")]
[Authorize(Roles = $"{Roles.Agent},{Roles.Supervisor},{Roles.Admin}")]
public sealed class KnowledgeBaseContentController : BaseApiController
{
    private readonly IMediator _mediator;

    public KnowledgeBaseContentController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(typeof(KnowledgeBaseContentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<KnowledgeBaseContentDto>> Create(
        [FromBody] CreateKnowledgeBaseContentRequest request, CancellationToken ct)
    {
        var command = new CreateKnowledgeBaseContentCommand(
            request.Title, request.Body, request.Summary, request.Type, request.Language, request.IsPublished);
        var created = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(KnowledgeBaseContentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<KnowledgeBaseContentDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetKnowledgeBaseContentByIdQuery(id), ct));

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<KnowledgeBaseContentListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<KnowledgeBaseContentListItemDto>>> GetList(
        [FromQuery] ContentType? type, [FromQuery] string? language, [FromQuery] bool? isPublished,
        [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = new GetKnowledgeBaseContentListQuery(type, language, isPublished, search, page, pageSize);
        return Ok(await _mediator.Send(query, ct));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(KnowledgeBaseContentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<KnowledgeBaseContentDto>> Update(
        Guid id, [FromBody] UpdateKnowledgeBaseContentRequest request, CancellationToken ct)
    {
        var command = new UpdateKnowledgeBaseContentCommand(
            id, request.Title, request.Body, request.Summary, request.Type, request.Language, request.IsPublished);
        return Ok(await _mediator.Send(command, ct));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteKnowledgeBaseContentCommand(id), ct);
        return NoContent();
    }
}
