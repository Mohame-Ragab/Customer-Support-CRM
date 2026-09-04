using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Features.Agents.Tasks.Commands;
using CustomerSupportCRM.Application.Features.Agents.Tasks.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Agents.Tasks.Queries;

public sealed class GetMyAgentTasksQueryHandler : IRequestHandler<GetMyAgentTasksQuery, IReadOnlyList<AgentTaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetMyAgentTasksQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public Task<IReadOnlyList<AgentTaskDto>> Handle(GetMyAgentTasksQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedException("Listing tasks requires an authenticated agent.");

        var query = _unitOfWork.Repository<AgentTask>().Query().Where(t => t.OwnerUserId == userId);
        if (!request.IncludeCompleted)
        {
            query = query.Where(t => !t.IsCompleted);
        }

        var tasks = query.OrderBy(t => t.DueAt).ToList();

        IReadOnlyList<AgentTaskDto> result = tasks.Select(CreateAgentTaskCommandHandler.ToDto).ToList();
        return Task.FromResult(result);
    }
}
