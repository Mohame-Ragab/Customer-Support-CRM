using AutoMapper;
using CustomerSupportCRM.Application.Features.KnowledgeBase.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.KnowledgeBase.Commands;

public sealed class UpdateKnowledgeBaseContentCommandHandler
    : IRequestHandler<UpdateKnowledgeBaseContentCommand, KnowledgeBaseContentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateKnowledgeBaseContentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<KnowledgeBaseContentDto> Handle(
        UpdateKnowledgeBaseContentCommand request, CancellationToken cancellationToken)
    {
        var content = await _unitOfWork.Repository<KnowledgeBaseContent>().GetByIdAsync(request.Id, cancellationToken);
        if (content is null)
        {
            throw new NotFoundException(nameof(KnowledgeBaseContent), request.Id);
        }

        content.Title = request.Title;
        content.Body = request.Body;
        content.Summary = request.Summary;
        content.Type = request.Type;
        content.Language = string.IsNullOrWhiteSpace(request.Language) ? content.Language : request.Language.ToLowerInvariant();
        content.IsPublished = request.IsPublished ?? content.IsPublished;

        _unitOfWork.Repository<KnowledgeBaseContent>().Update(content);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<KnowledgeBaseContentDto>(content);
    }
}
