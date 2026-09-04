using CustomerSupportCRM.Application.Features.Auth.Login;
using CustomerSupportCRM.Application.Common.Models;

namespace CustomerSupportCRM.Application.Common.Interfaces;

public interface IAuthenticationService
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken ct);
}
