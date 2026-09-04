using AutoMapper;
using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.CustomerPortal.Tickets;

public sealed class GetMyPortalTicketsQueryHandler : IRequestHandler<GetMyPortalTicketsQuery, PagedResult<TicketDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetMyPortalTicketsQueryHandler(
        IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<PagedResult<TicketDto>> Handle(GetMyPortalTicketsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedException("Listing tickets requires an authenticated customer.");
        var email = _currentUserService.Email;

        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        // Lookup-only (never creates) - a customer who never submitted
        // anything simply has no Customer CRM record yet; that is an empty
        // list, not an error. Matches by the reliable ApplicationUserId link
        // first; falls back to email only for a Customer row created before
        // that link existed and not yet backfilled (see ICustomerResolver).
        var customerId = _unitOfWork.Repository<Customer>().Query()
            .Where(c => c.ApplicationUserId == userId || (c.ApplicationUserId == null && c.Email == email))
            .Select(c => (Guid?)c.Id)
            .FirstOrDefault();

        if (customerId is null)
        {
            return new PagedResult<TicketDto>([], page, pageSize, 0);
        }

        var query = _unitOfWork.Repository<Ticket>().Query()
            .Where(t => t.CustomerId == customerId.Value);

        var totalCount = query.Count();

        var tickets = query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var categoryIds = tickets.Where(t => t.CategoryId.HasValue).Select(t => t.CategoryId!.Value).Distinct().ToList();
        if (categoryIds.Count > 0)
        {
            var categories = (await _unitOfWork.Repository<TicketCategory>()
                .FindAsync(c => categoryIds.Contains(c.Id), cancellationToken))
                .ToDictionary(c => c.Id);

            foreach (var ticket in tickets.Where(t => t.CategoryId.HasValue && categories.ContainsKey(t.CategoryId!.Value)))
            {
                ticket.Category = categories[ticket.CategoryId!.Value];
            }
        }

        var items = _mapper.Map<List<TicketDto>>(tickets);
        return new PagedResult<TicketDto>(items, page, pageSize, totalCount);
    }
}
