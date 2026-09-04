using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.QuickReplies;

public sealed class CreateQuickReplyTemplateCommandHandler
    : IRequestHandler<CreateQuickReplyTemplateCommand, QuickReplyTemplateDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateQuickReplyTemplateCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<QuickReplyTemplateDto> Handle(
        CreateQuickReplyTemplateCommand request, CancellationToken cancellationToken)
    {
        var ownerUserId = _currentUserService.UserId
            ?? throw new UnauthorizedException("Creating a quick reply requires an authenticated agent.");

        var nameTaken = await _unitOfWork.Repository<QuickReplyTemplate>().ExistsAsync(
            t => t.OwnerUserId == ownerUserId && t.Name == request.Name, cancellationToken);
        if (nameTaken)
        {
            throw new ConflictException($"A quick reply template named \"{request.Name}\" already exists.");
        }

        var template = new QuickReplyTemplate
        {
            OwnerUserId = ownerUserId,
            Name = request.Name,
            Body = request.Body,
        };

        await _unitOfWork.Repository<QuickReplyTemplate>().AddAsync(template, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(template);
    }

    internal static QuickReplyTemplateDto ToDto(QuickReplyTemplate t) =>
        new(t.Id, t.Name, t.Body, t.CreatedAt, t.UpdatedAt);
}
