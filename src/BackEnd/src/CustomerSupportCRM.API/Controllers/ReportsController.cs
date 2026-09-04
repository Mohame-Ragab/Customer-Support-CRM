using CustomerSupportCRM.Application.Features.Reports.AgentPerformance;
using CustomerSupportCRM.Application.Features.Reports.CustomerSatisfaction;
using CustomerSupportCRM.Application.Features.Reports.Tickets;
using CustomerSupportCRM.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupportCRM.API.Controllers;

/// <summary>
/// Read-only management reporting (F09 reports-management): ticket volume,
/// agent performance, and customer satisfaction, each over a caller-supplied
/// date range. Admin/Supervisor only - Agent and Customer are forbidden.
/// FR-047 (SLA Performance) was removed from scope; no SLA endpoint exists
/// or will be added here.
/// </summary>
[ApiController]
[Route("api/reports")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Supervisor}")]
public sealed class ReportsController : BaseApiController
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// GET /api/reports/tickets?fromDate=&amp;toDate= - total volume + breakdowns
    /// by status/category/priority (F09 ticket-reports).
    /// </summary>
    /// <remarks>
    /// Bound as individual scalar query parameters (not the query record
    /// directly) - see SearchKnowledgeBaseQuery (F06) for why: binding a
    /// record type via a single [FromQuery] parameter makes [ApiController]'s
    /// automatic model-state validation short-circuit with its own
    /// ValidationProblemDetails shape before FluentValidation ever runs. The
    /// handler validates explicitly instead, keeping the 400 shape consistent
    /// with every other endpoint.
    /// </remarks>
    [HttpGet("tickets")]
    [ProducesResponseType(typeof(TicketReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TicketReportResponse>> GetTicketReport(
        [FromQuery] DateOnly fromDate, [FromQuery] DateOnly toDate, CancellationToken ct)
        => Ok(await _mediator.Send(new GetTicketReportQuery(fromDate, toDate), ct));

    /// <summary>GET /api/reports/agent-performance?from=&amp;to=&amp;agentId= - per-agent tickets assigned/resolved and average resolution time (F09 agent-performance-reports).</summary>
    [HttpGet("agent-performance")]
    [ProducesResponseType(typeof(AgentPerformanceReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<AgentPerformanceReportDto>> GetAgentPerformance(
        [FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] Guid? agentId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetAgentPerformanceQuery(from, to, agentId), ct));

    /// <summary>GET /api/reports/customer-satisfaction?from=&amp;to= - average rating, response count, and rating distribution (F09 customer-satisfaction-reports). Always 200, even with zero feedback in range.</summary>
    [HttpGet("customer-satisfaction")]
    [ProducesResponseType(typeof(CustomerSatisfactionReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CustomerSatisfactionReportDto>> GetCustomerSatisfaction(
        [FromQuery] DateOnly from, [FromQuery] DateOnly to, CancellationToken ct)
        => Ok(await _mediator.Send(new GetCustomerSatisfactionReportQuery(from, to), ct));
}
