using MediatR;

namespace CustomerSupportCRM.Application.Features.Reports.AgentPerformance;

/// <summary>F09 reports-management/agent-performance-reports. Scalar-bound - see GetTicketReportQuery for the ValidationFilter rationale.</summary>
public sealed record GetAgentPerformanceQuery(DateTime From, DateTime To, Guid? AgentId)
    : IRequest<AgentPerformanceReportDto>;

public sealed record AgentPerformanceRowDto(
    Guid AgentId,
    string AgentUserName,
    int TicketsAssigned,
    int TicketsResolved,
    double? AverageResolutionHours);

public sealed record AgentPerformanceReportDto(DateTime From, DateTime To, IReadOnlyList<AgentPerformanceRowDto> Rows);
