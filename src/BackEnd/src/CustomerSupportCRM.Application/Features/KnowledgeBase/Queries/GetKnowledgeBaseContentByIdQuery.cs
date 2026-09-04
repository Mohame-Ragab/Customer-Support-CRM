using CustomerSupportCRM.Application.Features.KnowledgeBase.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.KnowledgeBase.Queries;

public sealed record GetKnowledgeBaseContentByIdQuery(Guid Id) : IRequest<KnowledgeBaseContentDto>;
