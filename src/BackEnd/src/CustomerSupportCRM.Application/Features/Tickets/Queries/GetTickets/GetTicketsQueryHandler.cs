using AutoMapper;
using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.Queries.GetTickets;

public sealed class GetTicketsQueryHandler : IRequestHandler<GetTicketsQuery, PagedResult<TicketDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetTicketsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<PagedResult<TicketDto>> Handle(GetTicketsQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var repository = _unitOfWork.Repository<Ticket>();
        var query = repository.Query();

        if (request.AssignedToMe)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("Filtering by assignedToMe requires an authenticated agent.");
            query = query.Where(t => t.AssignedAgentId == userId);
        }

        // Application cannot reference EF Core's ToListAsync (Onion layering);
        // synchronous ToList() still executes a single materialized query.
        var tickets = query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var totalCount = request.AssignedToMe
            ? query.Count()
            : await repository.CountAsync(cancellationToken: cancellationToken);

        // Batch-attach categories in-memory (no Include() available here - see
        // GetTicketByIdQueryHandler) to avoid an N+1 lookup per ticket.
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
