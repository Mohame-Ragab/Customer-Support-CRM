using CustomerSupportCRM.Application.Features.KnowledgeBase.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.KnowledgeBase.Queries;

/// <summary>
/// F08 customer-portal/access-faqs: same read as GetKnowledgeBaseContentByIdQuery
/// but additionally enforces IsPublished, so an anonymous/customer caller can
/// never fetch a draft by guessing its id (the staff query intentionally does
/// not check this, since staff must be able to view drafts).
/// </summary>
public sealed record GetPublishedKnowledgeBaseArticleQuery(Guid Id) : IRequest<KnowledgeBaseContentDto>;
