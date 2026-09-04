namespace CustomerSupportCRM.Application.Features.KnowledgeBase.Search;

public sealed record KnowledgeBaseSearchResultDto(
    Guid Id,
    string Title,
    string Excerpt,
    string Language,
    double Score,
    DateTime UpdatedAt);
