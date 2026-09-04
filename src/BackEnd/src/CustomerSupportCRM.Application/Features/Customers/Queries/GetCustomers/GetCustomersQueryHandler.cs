using AutoMapper;
using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.Customers.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Customers.Queries.GetCustomers;

public sealed class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, PagedResult<CustomerDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCustomersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var repository = _unitOfWork.Repository<Customer>();

        // Application cannot reference EF Core's ToListAsync (Onion layering);
        // synchronous ToList() still executes a single materialized query via
        // IQueryable, just without async I/O for this one round-trip.
        var customers = repository.Query()
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var totalCount = await repository.CountAsync(cancellationToken: cancellationToken);
        var items = _mapper.Map<List<CustomerDto>>(customers);

        return new PagedResult<CustomerDto>(items, page, pageSize, totalCount);
    }
}
