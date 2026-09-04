using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Features.Agents.Tasks.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Agents.Tasks.Commands;

public sealed class CreateAgentTaskCommandHandler : IRequestHandler<CreateAgentTaskCommand, AgentTaskDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateAgentTaskCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<AgentTaskDto> Handle(CreateAgentTaskCommand request, CancellationToken cancellationToken)
    {
        var ownerUserId = _currentUserService.UserId
            ?? throw new UnauthorizedException("Creating a task requires an authenticated agent.");

        var dueAt = NormalizeToUtc(request.DueAt);

        var task = new AgentTask
        {
            OwnerUserId = ownerUserId,
            Description = request.Description,
            DueAt = dueAt,
            IsCompleted = false,
            CompletedAt = null,
            TicketId = request.TicketId,
        };

        await _unitOfWork.Repository<AgentTask>().AddAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(task);
    }

    internal static DateTime NormalizeToUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc),
    };

    internal static AgentTaskDto ToDto(AgentTask task) => new(
        task.Id,
        task.Description,
        task.DueAt,
        task.IsCompleted,
        task.CompletedAt,
        IsOverdue: !task.IsCompleted && task.DueAt < DateTime.UtcNow,
        task.TicketId,
        task.CreatedAt,
        task.UpdatedAt);
}
