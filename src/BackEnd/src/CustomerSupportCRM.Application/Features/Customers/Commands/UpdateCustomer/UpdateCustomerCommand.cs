using CustomerSupportCRM.Application.Features.Customers.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Customers.Commands.UpdateCustomer;

public sealed record UpdateCustomerCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string? CompanyName,
    string? PreferredLanguage,
    string? Notes) : IRequest<CustomerDto>;
