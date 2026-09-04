using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.QuickReplies;

public sealed class GetQuickReplyTemplateByIdQueryHandler
    : IRequestHandler<GetQuickReplyTemplateByIdQuery, QuickReplyTemplateDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetQuickReplyTemplateByIdQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<QuickReplyTemplateDto> Handle(
        GetQuickReplyTemplateByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedException("Viewing a quick reply requires an authenticated agent.");

        var template = await _unitOfWork.Repository<QuickReplyTemplate>().GetByIdAsync(request.Id, cancellationToken);
        if (template is null || template.OwnerUserId != userId)
        {
            throw new NotFoundException(nameof(QuickReplyTemplate), request.Id);
        }

        return CreateQuickReplyTemplateCommandHandler.ToDto(template);
    }
}
