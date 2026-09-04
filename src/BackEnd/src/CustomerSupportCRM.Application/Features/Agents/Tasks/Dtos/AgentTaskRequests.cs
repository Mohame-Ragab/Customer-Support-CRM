namespace CustomerSupportCRM.Application.Features.Agents.Tasks.Dtos;

/// <summary>
/// Body-only request shapes (route id excluded where relevant) so
/// ValidationFilter - which only validates the actual MVC-bound
/// action-parameter type - actually runs these validators.
/// </summary>
public sealed record CreateAgentTaskRequest(string Description, DateTime DueAt, Guid? TicketId);

public sealed record UpdateAgentTaskRequest(string Description, DateTime DueAt, bool IsCompleted, Guid? TicketId);
