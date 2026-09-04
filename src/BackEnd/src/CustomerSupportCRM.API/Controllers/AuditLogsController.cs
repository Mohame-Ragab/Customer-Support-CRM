using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.Audit.Queries.GetAuditLogs;
using CustomerSupportCRM.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupportCRM.API.Controllers;

/// <summary>Admin-only, most-recent-first audit log viewer (security-admin/view-audit-logs).</summary>
[Authorize(Roles = Roles.Admin)]
[Route("api/audit-logs")]
public sealed class AuditLogsController : BaseApiController
{
    private readonly IMediator _mediator;

    public AuditLogsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AuditLogEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<AuditLogEntryDto>>> GetAuditLogs(
        [FromQuery] GetAuditLogsQuery query, CancellationToken ct)
        => Ok(await _mediator.Send(query, ct));
}
