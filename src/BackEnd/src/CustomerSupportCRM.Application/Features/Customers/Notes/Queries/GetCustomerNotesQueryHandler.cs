using CustomerSupportCRM.Application.Features.Customers.Notes.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Customers.Notes.Queries;

public sealed class GetCustomerNotesQueryHandler : IRequestHandler<GetCustomerNotesQuery, IReadOnlyList<CustomerNoteDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCustomerNotesQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<CustomerNoteDto>> Handle(GetCustomerNotesQuery request, CancellationToken cancellationToken)
    {
        var customerExists = await _unitOfWork.Repository<Customer>()
            .ExistsAsync(c => c.Id == request.CustomerId, cancellationToken);

        if (!customerExists)
        {
            throw new NotFoundException(nameof(Customer), request.CustomerId);
        }

        var notes = await _unitOfWork.Repository<CustomerNote>()
            .FindAsync(n => n.CustomerId == request.CustomerId, cancellationToken);

        return notes
            .OrderBy(n => n.CreatedAt)
            .Select(n => new CustomerNoteDto(n.Id, n.CustomerId, n.AuthorUserId, n.Content, n.CreatedAt))
            .ToList();
    }
}
