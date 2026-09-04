using CustomerSupportCRM.Application.Common.Models;
using MediatR;

namespace CustomerSupportCRM.Application.Features.KnowledgeBase.Search;

/// <summary>
/// Bound directly as [FromQuery] on the controller action (no route id
/// involved), so ValidationFilter - which validates the actual MVC-bound
/// action-parameter type - runs SearchKnowledgeBaseQueryValidator automatically.
/// </summary>
public sealed record SearchKnowledgeBaseQuery(
    string Query, string? Language = null, int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<KnowledgeBaseSearchResultDto>>;
