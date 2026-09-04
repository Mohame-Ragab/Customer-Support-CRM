using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.QuickReplies;

public sealed class UpdateQuickReplyTemplateCommandHandler
    : IRequestHandler<UpdateQuickReplyTemplateCommand, QuickReplyTemplateDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateQuickReplyTemplateCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<QuickReplyTemplateDto> Handle(
        UpdateQuickReplyTemplateCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedException("Updating a quick reply requires an authenticated agent.");

        // Non-owner gets 404, not 403 - do not confirm existence of another agent's template.
        var template = await _unitOfWork.Repository<QuickReplyTemplate>().GetByIdAsync(request.Id, cancellationToken);
        if (template is null || template.OwnerUserId != userId)
        {
            throw new NotFoundException(nameof(QuickReplyTemplate), request.Id);
        }

        var nameTaken = await _unitOfWork.Repository<QuickReplyTemplate>().ExistsAsync(
            t => t.OwnerUserId == userId && t.Name == request.Name && t.Id != request.Id, cancellationToken);
        if (nameTaken)
        {
            throw new ConflictException($"A quick reply template named \"{request.Name}\" already exists.");
        }

        template.Name = request.Name;
        template.Body = request.Body;

        _unitOfWork.Repository<QuickReplyTemplate>().Update(template);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CreateQuickReplyTemplateCommandHandler.ToDto(template);
    }
}
