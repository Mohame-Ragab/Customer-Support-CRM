namespace CustomerSupportCRM.Application.Features.Tickets.Dtos;

/// <summary>POST /api/tickets/{id}/escalate request body. The ticket id comes from the route, not this body.</summary>
public sealed record EscalateTicketRequest(string? Reason);
