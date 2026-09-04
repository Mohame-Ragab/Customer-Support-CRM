using CustomerSupportCRM.Domain.Enums;

namespace CustomerSupportCRM.Application.Features.Tickets.Dtos;

public sealed record TicketMessageDto(
    Guid Id,
    Guid TicketId,
    TicketChannel Channel,
    TicketMessageDirection Direction,
    EmailDeliveryStatus DeliveryStatus,
    string FromAddress,
    string ToAddress,
    string? Cc,
    string Subject,
    string BodyText,
    string? BodyHtml,
    string? FailureReason,
    Guid? SentByUserId,
    DateTime CreatedAt);
