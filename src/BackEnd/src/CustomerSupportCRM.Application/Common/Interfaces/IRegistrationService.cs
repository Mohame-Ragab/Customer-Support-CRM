using CustomerSupportCRM.Application.Features.Auth.Commands.RegisterCustomer;

namespace CustomerSupportCRM.Application.Common.Interfaces;

public interface IRegistrationService
{
    Task<(bool Success, RegisterCustomerResponse? Response, string? ErrorKey)> RegisterAsync(
        RegisterCustomerCommand command,
        CancellationToken cancellationToken);
}
