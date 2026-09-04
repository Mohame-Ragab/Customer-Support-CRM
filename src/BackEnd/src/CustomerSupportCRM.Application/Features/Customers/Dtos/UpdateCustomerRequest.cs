namespace CustomerSupportCRM.Application.Features.Customers.Dtos;

/// <summary>PUT /api/customers/{id} request body. The customer id comes from the route, not this body - see Commands/UpdateCustomer/UpdateCustomerCommand.cs, which the controller assembles from both.</summary>
public sealed record UpdateCustomerRequest(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string? CompanyName,
    string? PreferredLanguage,
    string? Notes) : IHasCustomerContactFields;
