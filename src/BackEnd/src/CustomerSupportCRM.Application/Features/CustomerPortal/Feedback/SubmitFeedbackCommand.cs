using MediatR;

namespace CustomerSupportCRM.Application.Features.CustomerPortal.Feedback;

public sealed record SubmitFeedbackCommand(Guid TicketId, int Rating, string? Comment) : IRequest<SubmitFeedbackResponse>;

public sealed record SubmitFeedbackResponse(Guid FeedbackId, DateTime SubmittedAt);

/// <summary>Body-only shape bound via [FromBody] - route supplies TicketId (same ValidationFilter rationale as F02/F03/F04 request DTOs).</summary>
public sealed record SubmitFeedbackRequest(int Rating, string? Comment);
