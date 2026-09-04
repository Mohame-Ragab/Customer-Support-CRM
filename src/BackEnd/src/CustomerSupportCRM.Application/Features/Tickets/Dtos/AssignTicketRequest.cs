namespace CustomerSupportCRM.Application.Features.Tickets.Dtos;

/// <summary>POST /api/tickets/{id}/assign request body. The ticket id comes from the route, not this body.</summary>
public sealed record AssignTicketRequest(Guid AgentUserId);
