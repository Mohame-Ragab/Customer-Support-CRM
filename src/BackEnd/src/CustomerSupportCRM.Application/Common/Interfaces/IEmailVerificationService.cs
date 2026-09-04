using CustomerSupportCRM.Application.Common.Models;

namespace CustomerSupportCRM.Application.Common.Interfaces;

public interface IEmailVerificationService
{
    Task<Result> ConfirmAsync(Guid userId, string token, CancellationToken ct);
    Task<Result> ResendAsync(string email, CancellationToken ct);
}
