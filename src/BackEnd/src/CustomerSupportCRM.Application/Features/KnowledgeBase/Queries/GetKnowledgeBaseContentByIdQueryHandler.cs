using AutoMapper;
using CustomerSupportCRM.Application.Features.KnowledgeBase.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.KnowledgeBase.Queries;

public sealed class GetKnowledgeBaseContentByIdQueryHandler
    : IRequestHandler<GetKnowledgeBaseContentByIdQuery, KnowledgeBaseContentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetKnowledgeBaseContentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<KnowledgeBaseContentDto> Handle(
        GetKnowledgeBaseContentByIdQuery request, CancellationToken cancellationToken)
    {
        var content = await _unitOfWork.Repository<KnowledgeBaseContent>().GetByIdAsync(request.Id, cancellationToken);
        if (content is null)
        {
            throw new NotFoundException(nameof(KnowledgeBaseContent), request.Id);
        }

        return _mapper.Map<KnowledgeBaseContentDto>(content);
    }
}
