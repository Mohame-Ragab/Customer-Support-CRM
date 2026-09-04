using MediatR;

namespace CustomerSupportCRM.Application.Features.Channels.WebForms;

/// <summary>
/// Public web-form submission (F03 web-forms-channel). A MediatR command, not a
/// plain DTO + hand-rolled handler interface (the plan's suggestion) - matches
/// this codebase's actual, consistently-used CQRS convention.
/// </summary>
public sealed record SubmitWebFormRequest(
    string SubmitterName,
    string SubmitterEmail,
    string Subject,
    string Message,
    string? Category,
    /// <summary>Honeypot field. Must be null/empty; a non-empty value is treated as spam.</summary>
    string? Honeypot) : IRequest<SubmitWebFormResponse>;

public sealed record SubmitWebFormResponse(Guid TicketId, string TrackingCode);
