using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.QuickReplies;

public sealed class GetMyQuickReplyTemplatesQueryHandler
    : IRequestHandler<GetMyQuickReplyTemplatesQuery, IReadOnlyList<QuickReplyTemplateDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetMyQuickReplyTemplatesQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public Task<IReadOnlyList<QuickReplyTemplateDto>> Handle(
        GetMyQuickReplyTemplatesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedException("Listing quick replies requires an authenticated agent.");

        var query = _unitOfWork.Repository<QuickReplyTemplate>().Query().Where(t => t.OwnerUserId == userId);

        var search = request.Search?.Trim();
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(t => t.Name.Contains(search));
        }

        // No pagination in v1 - a personal template list; capped defensively.
        var templates = query.OrderBy(t => t.Name).Take(200).ToList();

        IReadOnlyList<QuickReplyTemplateDto> result =
            templates.Select(CreateQuickReplyTemplateCommandHandler.ToDto).ToList();
        return Task.FromResult(result);
    }
}
