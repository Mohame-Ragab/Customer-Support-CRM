using CustomerSupportCRM.Domain.Enums;

namespace CustomerSupportCRM.Application.Features.KnowledgeBase.Dtos;

/// <summary>
/// Body-only request shapes (route id excluded on update) so ValidationFilter -
/// which only validates the actual MVC-bound action-parameter type - runs
/// these validators. Same pattern as F02's SetTicketClassificationRequest.
/// </summary>
public sealed record CreateKnowledgeBaseContentRequest(
    string Title, string Body, string? Summary, ContentType Type, string? Language, bool? IsPublished);

public sealed record UpdateKnowledgeBaseContentRequest(
    string Title, string Body, string? Summary, ContentType Type, string? Language, bool? IsPublished);
