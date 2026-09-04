using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Features.Agents.Tasks.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Agents.Tasks.Commands;

public sealed class UpdateAgentTaskCommandHandler : IRequestHandler<UpdateAgentTaskCommand, AgentTaskDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateAgentTaskCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<AgentTaskDto> Handle(UpdateAgentTaskCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedException("Updating a task requires an authenticated agent.");

        var task = await _unitOfWork.Repository<AgentTask>().GetByIdAsync(request.Id, cancellationToken);
        if (task is null)
        {
            throw new NotFoundException(nameof(AgentTask), request.Id);
        }

        if (task.OwnerUserId != userId)
        {
            throw new ForbiddenAccessException("You can only modify your own tasks.");
        }

        task.Description = request.Description;
        task.DueAt = CreateAgentTaskCommandHandler.NormalizeToUtc(request.DueAt);
        task.TicketId = request.TicketId;

        if (request.IsCompleted && !task.IsCompleted)
        {
            task.CompletedAt = DateTime.UtcNow;
        }
        else if (!request.IsCompleted && task.IsCompleted)
        {
            task.CompletedAt = null;
        }
        task.IsCompleted = request.IsCompleted;

        _unitOfWork.Repository<AgentTask>().Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CreateAgentTaskCommandHandler.ToDto(task);
    }
}
