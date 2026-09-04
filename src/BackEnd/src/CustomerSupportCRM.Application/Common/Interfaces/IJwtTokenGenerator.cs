namespace CustomerSupportCRM.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAtUtc) GenerateAccessToken(
        Guid userId, string email, string displayName, IReadOnlyCollection<string> roles);

    string GenerateRefreshToken();
}
