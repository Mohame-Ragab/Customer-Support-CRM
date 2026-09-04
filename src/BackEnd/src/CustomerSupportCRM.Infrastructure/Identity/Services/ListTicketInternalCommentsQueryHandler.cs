using CustomerSupportCRM.Application.Features.Tickets.InternalComments;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CustomerSupportCRM.Infrastructure.Identity.Services;

/// <summary>
/// Lives in Infrastructure (not Application) because it resolves each
/// comment's author display name via <see cref="UserManager{TUser}"/> -
/// Identity types are unavailable to the Application layer. Same rationale as
/// AssignTicketCommandHandler (F02).
/// </summary>
public sealed class ListTicketInternalCommentsQueryHandler
    : IRequestHandler<ListTicketInternalCommentsQuery, IReadOnlyList<TicketInternalCommentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public ListTicketInternalCommentsQueryHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<IReadOnlyList<TicketInternalCommentDto>> Handle(
        ListTicketInternalCommentsQuery request, CancellationToken cancellationToken)
    {
        var ticketExists = await _unitOfWork.Repository<Ticket>()
            .ExistsAsync(t => t.Id == request.TicketId, cancellationToken);
        if (!ticketExists)
        {
            throw new NotFoundException(nameof(Ticket), request.TicketId);
        }

        var comments = _unitOfWork.Repository<TicketInternalComment>().Query()
            .Where(c => c.TicketId == request.TicketId)
            .OrderBy(c => c.CreatedAt)
            .ToList();

        var authorIds = comments
            .Where(c => c.CreatedBy != null)
            .Select(c => c.CreatedBy!)
            .Distinct()
            .ToList();

        var displayNames = new Dictionary<string, string?>();
        foreach (var authorId in authorIds)
        {
            if (Guid.TryParse(authorId, out var userId))
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                // "Deleted user" fallback is applied on the frontend when this is null.
                displayNames[authorId] = user?.FullName ?? user?.Email;
            }
        }

        return comments.Select(c => new TicketInternalCommentDto(
            c.Id, c.TicketId, c.Body, c.CreatedBy,
            c.CreatedBy != null && displayNames.TryGetValue(c.CreatedBy, out var name) ? name : null,
            c.CreatedAt)).ToList();
    }
}
