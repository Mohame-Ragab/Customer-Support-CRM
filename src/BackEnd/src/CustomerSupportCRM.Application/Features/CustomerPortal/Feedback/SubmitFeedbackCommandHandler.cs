using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Enums;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.CustomerPortal.Feedback;

public sealed class SubmitFeedbackCommandHandler : IRequestHandler<SubmitFeedbackCommand, SubmitFeedbackResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public SubmitFeedbackCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<SubmitFeedbackResponse> Handle(SubmitFeedbackCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedException("Submitting feedback requires an authenticated customer.");
        var email = _currentUserService.Email;

        var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(request.TicketId, cancellationToken);
        if (ticket is null)
        {
            throw new NotFoundException(nameof(Ticket), request.TicketId);
        }

        // Same NotFoundException whether the ticket doesn't exist or simply isn't
        // this customer's - never confirms existence of another customer's ticket.
        // ApplicationUserId is the reliable match; email is a fallback for a
        // Customer row not yet backfilled with the link (see ICustomerResolver).
        var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(ticket.CustomerId, cancellationToken);
        var isOwner = customer is not null && (customer.ApplicationUserId.HasValue
            ? customer.ApplicationUserId == userId
            : email is not null && string.Equals(customer.Email, email, StringComparison.OrdinalIgnoreCase));
        if (!isOwner)
        {
            throw new NotFoundException(nameof(Ticket), request.TicketId);
        }

        if (ticket.Status is not (TicketStatus.Resolved or TicketStatus.Closed))
        {
            throw new ConflictException("Feedback can only be submitted for a resolved or closed ticket.");
        }

        var alreadySubmitted = await _unitOfWork.Repository<CustomerFeedback>().ExistsAsync(
            f => f.TicketId == request.TicketId && f.CustomerUserId == userId, cancellationToken);
        if (alreadySubmitted)
        {
            throw new ConflictException("Feedback has already been submitted for this ticket.");
        }

        var feedback = new CustomerFeedback
        {
            TicketId = request.TicketId,
            CustomerUserId = userId,
            Rating = request.Rating,
            Comment = request.Comment,
            SubmittedAt = DateTime.UtcNow,
        };

        await _unitOfWork.Repository<CustomerFeedback>().AddAsync(feedback, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SubmitFeedbackResponse(feedback.Id, feedback.SubmittedAt);
    }
}
