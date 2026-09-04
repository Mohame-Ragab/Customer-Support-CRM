namespace CustomerSupportCRM.Application.Features.Customers.Dtos;

/// <summary>Shared read contract for a customer profile - used by create, view, and update responses (customers/create-customer, view-customer, update-customer).</summary>
public sealed record CustomerDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string? CompanyName,
    string? PreferredLanguage,
    string? Notes,
    DateTime CreatedAt,
    string? CreatedBy,
    DateTime? UpdatedAt);
