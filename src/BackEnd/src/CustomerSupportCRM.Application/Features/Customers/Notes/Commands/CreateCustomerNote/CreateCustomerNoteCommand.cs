using CustomerSupportCRM.Application.Features.Customers.Notes.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Customers.Notes.Commands.CreateCustomerNote;

public sealed record CreateCustomerNoteCommand(Guid CustomerId, string Content) : IRequest<CustomerNoteDto>;
