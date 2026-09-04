namespace CustomerSupportCRM.Application.Common.Interfaces;

public interface IRefreshTokenService
{
    Task<(string RawToken, DateTime ExpiresAt)> IssueAsync(
        string userId, string? ip, CancellationToken ct);

    Task<(string UserId, string NewRawToken, DateTime NewExpiresAt)> RotateAsync(
        string presentedRawToken, string? ip, CancellationToken ct);

    Task RevokeAsync(string presentedRawToken, string? ip, CancellationToken ct);
}
