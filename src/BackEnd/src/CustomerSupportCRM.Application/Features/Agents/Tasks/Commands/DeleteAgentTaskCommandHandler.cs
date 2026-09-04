using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Agents.Tasks.Commands;

public sealed class DeleteAgentTaskCommandHandler : IRequestHandler<DeleteAgentTaskCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteAgentTaskCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteAgentTaskCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedException("Deleting a task requires an authenticated agent.");

        var task = await _unitOfWork.Repository<AgentTask>().GetByIdAsync(request.Id, cancellationToken);
        if (task is null)
        {
            throw new NotFoundException(nameof(AgentTask), request.Id);
        }

        if (task.OwnerUserId != userId)
        {
            throw new ForbiddenAccessException("You can only delete your own tasks.");
        }

        _unitOfWork.Repository<AgentTask>().Delete(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
