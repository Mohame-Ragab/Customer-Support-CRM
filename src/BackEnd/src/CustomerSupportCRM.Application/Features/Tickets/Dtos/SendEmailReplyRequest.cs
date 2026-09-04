namespace CustomerSupportCRM.Application.Features.Tickets.Dtos;

/// <summary>
/// Body-only shape of <c>SendEmailReplyCommand</c> (excludes the route-derived
/// TicketId) so <c>ValidationFilter</c> - which only validates the actual
/// MVC-bound action-parameter type, not a MediatR command assembled inside the
/// controller - actually runs this DTO's validator. Same pattern as F02's
/// SetTicketClassificationRequest/AssignTicketRequest/etc.
/// </summary>
public sealed record SendEmailReplyRequest(string Subject, string BodyText, string? BodyHtml);
