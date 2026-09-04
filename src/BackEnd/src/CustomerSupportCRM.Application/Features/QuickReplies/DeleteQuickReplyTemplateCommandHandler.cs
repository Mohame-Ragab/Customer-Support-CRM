using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.QuickReplies;

public sealed class DeleteQuickReplyTemplateCommandHandler : IRequestHandler<DeleteQuickReplyTemplateCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteQuickReplyTemplateCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteQuickReplyTemplateCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedException("Deleting a quick reply requires an authenticated agent.");

        var template = await _unitOfWork.Repository<QuickReplyTemplate>().GetByIdAsync(request.Id, cancellationToken);
        if (template is null || template.OwnerUserId != userId)
        {
            throw new NotFoundException(nameof(QuickReplyTemplate), request.Id);
        }

        _unitOfWork.Repository<QuickReplyTemplate>().Delete(template);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
