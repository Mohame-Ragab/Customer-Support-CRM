using AutoMapper;
using CustomerSupportCRM.Application.Features.KnowledgeBase.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.KnowledgeBase.Commands;

public sealed class CreateKnowledgeBaseContentCommandHandler
    : IRequestHandler<CreateKnowledgeBaseContentCommand, KnowledgeBaseContentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateKnowledgeBaseContentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<KnowledgeBaseContentDto> Handle(
        CreateKnowledgeBaseContentCommand request, CancellationToken cancellationToken)
    {
        var content = new KnowledgeBaseContent
        {
            Title = request.Title,
            Body = request.Body,
            Summary = request.Summary,
            Type = request.Type,
            Language = string.IsNullOrWhiteSpace(request.Language) ? "en" : request.Language.ToLowerInvariant(),
            IsPublished = request.IsPublished ?? true,
        };

        await _unitOfWork.Repository<KnowledgeBaseContent>().AddAsync(content, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<KnowledgeBaseContentDto>(content);
    }
}
