using AutoMapper;
using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.KnowledgeBase.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.KnowledgeBase.Queries;

public sealed class GetKnowledgeBaseContentListQueryHandler
    : IRequestHandler<GetKnowledgeBaseContentListQuery, PagedResult<KnowledgeBaseContentListItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetKnowledgeBaseContentListQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public Task<PagedResult<KnowledgeBaseContentListItemDto>> Handle(
        GetKnowledgeBaseContentListQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var query = _unitOfWork.Repository<KnowledgeBaseContent>().Query();

        if (request.Type.HasValue)
        {
            query = query.Where(c => c.Type == request.Type.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Language))
        {
            var language = request.Language.ToLowerInvariant();
            query = query.Where(c => c.Language == language);
        }

        if (request.IsPublished.HasValue)
        {
            query = query.Where(c => c.IsPublished == request.IsPublished.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(c => c.Title.Contains(search) || c.Body.Contains(search));
        }

        var totalCount = query.Count();

        var items = query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var dtos = _mapper.Map<List<KnowledgeBaseContentListItemDto>>(items);
        return Task.FromResult(new PagedResult<KnowledgeBaseContentListItemDto>(dtos, page, pageSize, totalCount));
    }
}
