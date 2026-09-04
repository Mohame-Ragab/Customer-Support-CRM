using CustomerSupportCRM.Application.Features.Agents.Tasks.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Agents.Tasks.Commands;

public sealed record UpdateAgentTaskCommand(
    Guid Id, string Description, DateTime DueAt, bool IsCompleted, Guid? TicketId) : IRequest<AgentTaskDto>;
