using AutoMapper;
using CustomerSupportCRM.Application.Features.KnowledgeBase.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.KnowledgeBase.Queries;

public sealed class GetPublishedKnowledgeBaseArticleQueryHandler
    : IRequestHandler<GetPublishedKnowledgeBaseArticleQuery, KnowledgeBaseContentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPublishedKnowledgeBaseArticleQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<KnowledgeBaseContentDto> Handle(
        GetPublishedKnowledgeBaseArticleQuery request, CancellationToken cancellationToken)
    {
        var content = await _unitOfWork.Repository<KnowledgeBaseContent>().GetByIdAsync(request.Id, cancellationToken);
        if (content is null || !content.IsPublished)
        {
            throw new NotFoundException(nameof(KnowledgeBaseContent), request.Id);
        }

        return _mapper.Map<KnowledgeBaseContentDto>(content);
    }
}
