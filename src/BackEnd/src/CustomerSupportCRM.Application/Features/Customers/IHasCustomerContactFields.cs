namespace CustomerSupportCRM.Application.Features.Customers;

/// <summary>
/// Shared shape for the customer contact fields accepted by both create and
/// update requests, so their validation rules are defined once
/// (see Validators/CustomerFieldValidationRules.cs) rather than duplicated -
/// Plan 26 (update-customer) explicitly requires reusing Plan 24's rules
/// verbatim.
/// </summary>
public interface IHasCustomerContactFields
{
    string FirstName { get; }
    string LastName { get; }
    string Email { get; }
    string? PhoneNumber { get; }
    string? CompanyName { get; }
    string? PreferredLanguage { get; }
}
