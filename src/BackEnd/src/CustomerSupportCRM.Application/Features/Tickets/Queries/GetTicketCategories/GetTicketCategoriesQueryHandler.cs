using AutoMapper;
using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.Queries.GetTicketCategories;

public sealed class GetTicketCategoriesQueryHandler
    : IRequestHandler<GetTicketCategoriesQuery, IReadOnlyList<TicketCategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTicketCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TicketCategoryDto>> Handle(
        GetTicketCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _unitOfWork.Repository<TicketCategory>()
            .FindAsync(c => c.IsActive, cancellationToken);

        return categories
            .OrderBy(c => c.NameEn, StringComparer.Ordinal)
            .Select(c => _mapper.Map<TicketCategoryDto>(c))
            .ToList();
    }
}
