using CustomerSupportCRM.Application.Features.Agents.Tasks.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Agents.Tasks.Queries;

public sealed record GetMyAgentTasksQuery(bool IncludeCompleted = false) : IRequest<IReadOnlyList<AgentTaskDto>>;
