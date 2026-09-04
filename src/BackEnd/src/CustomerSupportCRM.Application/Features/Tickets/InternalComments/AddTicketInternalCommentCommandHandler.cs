using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.InternalComments;

public sealed class AddTicketInternalCommentCommandHandler
    : IRequestHandler<AddTicketInternalCommentCommand, TicketInternalCommentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public AddTicketInternalCommentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<TicketInternalCommentDto> Handle(
        AddTicketInternalCommentCommand request, CancellationToken cancellationToken)
    {
        var ticketExists = await _unitOfWork.Repository<Ticket>()
            .ExistsAsync(t => t.Id == request.TicketId, cancellationToken);
        if (!ticketExists)
        {
            throw new NotFoundException(nameof(Ticket), request.TicketId);
        }

        if (_currentUserService.UserId is null)
        {
            throw new UnauthorizedException("Posting an internal comment requires an authenticated user.");
        }

        var comment = new TicketInternalComment
        {
            TicketId = request.TicketId,
            Body = request.Body,
        };

        await _unitOfWork.Repository<TicketInternalComment>().AddAsync(comment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // The acting user's own display name is already known - no Identity lookup needed
        // (contrast with ListTicketInternalCommentsQueryHandler, which resolves other
        // authors' names and therefore lives in Infrastructure).
        return new TicketInternalCommentDto(
            comment.Id, comment.TicketId, comment.Body,
            comment.CreatedBy, _currentUserService.UserName, comment.CreatedAt);
    }
}
