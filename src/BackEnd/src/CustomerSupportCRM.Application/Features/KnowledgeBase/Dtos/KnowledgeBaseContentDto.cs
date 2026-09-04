using CustomerSupportCRM.Domain.Enums;

namespace CustomerSupportCRM.Application.Features.KnowledgeBase.Dtos;

/// <summary>Full content, including Body - used by create/update/get-by-id.</summary>
public sealed record KnowledgeBaseContentDto(
    Guid Id,
    string Title,
    string Body,
    string? Summary,
    ContentType Type,
    string Language,
    bool IsPublished,
    DateTime CreatedAt,
    string? CreatedBy,
    DateTime? UpdatedAt);

/// <summary>List-row projection - omits Body (can be up to nvarchar(max)) to keep list responses light.</summary>
public sealed record KnowledgeBaseContentListItemDto(
    Guid Id,
    string Title,
    string? Summary,
    ContentType Type,
    string Language,
    bool IsPublished,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
