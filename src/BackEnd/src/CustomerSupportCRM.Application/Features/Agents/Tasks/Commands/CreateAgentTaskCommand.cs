using CustomerSupportCRM.Application.Features.Agents.Tasks.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Agents.Tasks.Commands;

public sealed record CreateAgentTaskCommand(string Description, DateTime DueAt, Guid? TicketId) : IRequest<AgentTaskDto>;
