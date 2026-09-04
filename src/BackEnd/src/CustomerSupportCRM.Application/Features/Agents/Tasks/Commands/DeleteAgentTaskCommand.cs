using MediatR;

namespace CustomerSupportCRM.Application.Features.Agents.Tasks.Commands;

public sealed record DeleteAgentTaskCommand(Guid Id) : IRequest;
