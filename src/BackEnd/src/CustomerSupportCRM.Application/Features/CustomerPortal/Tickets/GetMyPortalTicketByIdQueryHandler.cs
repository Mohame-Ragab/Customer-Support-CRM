using AutoMapper;
using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Features.Tickets;
using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.CustomerPortal.Tickets;

public sealed class GetMyPortalTicketByIdQueryHandler : IRequestHandler<GetMyPortalTicketByIdQuery, TicketDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetMyPortalTicketByIdQueryHandler(
        IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<TicketDto> Handle(GetMyPortalTicketByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedException("Viewing a ticket requires an authenticated customer.");
        var email = _currentUserService.Email;

        var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(request.Id, cancellationToken);

        // Same NotFoundException for "doesn't exist" and "exists but isn't
        // yours" - never confirms/denies existence of another customer's ticket.
        if (ticket is null || !await IsOwnedByCurrentCustomerAsync(ticket, userId, email, cancellationToken))
        {
            throw new NotFoundException(nameof(Ticket), request.Id);
        }

        await ticket.AttachCategoryIfNeededAsync(_unitOfWork, cancellationToken);
        return _mapper.Map<TicketDto>(ticket);
    }

    private async Task<bool> IsOwnedByCurrentCustomerAsync(Ticket ticket, Guid userId, string? email, CancellationToken ct)
    {
        // ApplicationUserId is the reliable match; email is a fallback for a
        // Customer row not yet backfilled with the link (see ICustomerResolver).
        var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(ticket.CustomerId, ct);
        if (customer is null)
        {
            return false;
        }

        if (customer.ApplicationUserId.HasValue)
        {
            return customer.ApplicationUserId == userId;
        }

        return email is not null && string.Equals(customer.Email, email, StringComparison.OrdinalIgnoreCase);
    }
}
