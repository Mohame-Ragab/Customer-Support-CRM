namespace CustomerSupportCRM.Application.Features.Auth.Commands.RegisterCustomer;

public sealed record RegisterCustomerCommand(
    string Email,
    string Password);

public sealed record RegisterCustomerResponse(Guid UserId, string Email);
