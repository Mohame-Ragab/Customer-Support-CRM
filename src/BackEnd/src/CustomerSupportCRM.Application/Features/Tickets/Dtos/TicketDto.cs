using CustomerSupportCRM.Domain.Enums;

namespace CustomerSupportCRM.Application.Features.Tickets.Dtos;

/// <summary>Shared read contract for a ticket - used by create, list, and detail responses across all F02 stories.</summary>
public sealed record TicketDto(
    Guid Id,
    string Subject,
    string? Description,
    TicketStatus Status,
    Guid CustomerId,
    Guid? CategoryId,
    string? CategoryCode,
    TicketPriority? Priority,
    Guid? AssignedAgentId,
    DateTime? AssignedAt,
    bool IsEscalated,
    DateTime? EscalatedAt,
    TicketChannel Channel,
    DateTime CreatedAt,
    string? CreatedBy,
    DateTime? UpdatedAt);
