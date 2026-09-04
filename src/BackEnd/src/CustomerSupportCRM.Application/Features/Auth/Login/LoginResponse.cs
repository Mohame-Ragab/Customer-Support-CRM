namespace CustomerSupportCRM.Application.Features.Auth.Login;

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc,
    string TokenType,
    Guid UserId,
    string Email,
    string DisplayName,
    IReadOnlyList<string> Roles,
    DateTime RefreshTokenExpiresAtUtc);
