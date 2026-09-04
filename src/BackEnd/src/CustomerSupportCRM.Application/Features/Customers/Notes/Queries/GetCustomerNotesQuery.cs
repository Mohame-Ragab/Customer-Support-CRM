using CustomerSupportCRM.Application.Features.Customers.Notes.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Customers.Notes.Queries;

public sealed record GetCustomerNotesQuery(Guid CustomerId) : IRequest<IReadOnlyList<CustomerNoteDto>>;
