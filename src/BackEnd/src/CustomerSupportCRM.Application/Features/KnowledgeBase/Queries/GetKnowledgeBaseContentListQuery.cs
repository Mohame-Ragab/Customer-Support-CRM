using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.KnowledgeBase.Dtos;
using CustomerSupportCRM.Domain.Enums;
using MediatR;

namespace CustomerSupportCRM.Application.Features.KnowledgeBase.Queries;

public sealed record GetKnowledgeBaseContentListQuery(
    ContentType? Type = null,
    string? Language = null,
    bool? IsPublished = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<KnowledgeBaseContentListItemDto>>;
