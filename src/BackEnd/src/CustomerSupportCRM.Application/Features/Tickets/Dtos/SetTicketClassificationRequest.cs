using CustomerSupportCRM.Domain.Enums;

namespace CustomerSupportCRM.Application.Features.Tickets.Dtos;

/// <summary>PATCH /api/tickets/{id}/classification request body. The ticket id comes from the route, not this body.</summary>
public sealed record SetTicketClassificationRequest(Guid? CategoryId, TicketPriority? Priority);
