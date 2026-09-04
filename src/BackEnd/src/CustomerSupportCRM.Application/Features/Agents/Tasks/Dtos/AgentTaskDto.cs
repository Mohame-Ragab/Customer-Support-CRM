namespace CustomerSupportCRM.Application.Features.Agents.Tasks.Dtos;

public sealed record AgentTaskDto(
    Guid Id,
    string Description,
    DateTime DueAt,
    bool IsCompleted,
    DateTime? CompletedAt,
    bool IsOverdue,
    Guid? TicketId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
