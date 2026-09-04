namespace CustomerSupportCRM.Application.Common.Models;

/// <summary>
/// Strongly typed binding of the "Jwt" configuration section. Lives in Application
/// so both API (authentication middleware) and, later, Infrastructure (token
/// issuing service) can depend on it without either depending on the other.
/// </summary>
public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    /// <summary>Signing key. Must be supplied via environment variables/user-secrets/a secret store — never committed.</summary>
    public string SecretKey { get; init; } = string.Empty;

    public int ExpirationMinutes { get; init; } = 60;

    public int AccessTokenMinutes { get; init; } = 60;

    public int RefreshTokenDays { get; init; } = 14;
}
