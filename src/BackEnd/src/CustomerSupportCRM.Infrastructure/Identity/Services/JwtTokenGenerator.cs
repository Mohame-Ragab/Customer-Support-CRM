using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Common.Models;
using Microsoft.IdentityModel.Tokens;

namespace CustomerSupportCRM.Infrastructure.Identity.Services;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenGenerator(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    public (string Token, DateTime ExpiresAtUtc) GenerateAccessToken(
        Guid userId, string email, string displayName, IReadOnlyCollection<string> roles)
    {
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(_jwtSettings.AccessTokenMinutes);

        var claims = new List<System.Security.Claims.Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new("name", displayName ?? email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        // Add one "role" claim per role (plain string key, not ClaimTypes.Role URI)
        foreach (var role in roles)
        {
            claims.Add(new("role", role));
        }

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenString = tokenHandler.WriteToken(token);

        return (tokenString, expiresAt);
    }

    public string GenerateRefreshToken()
    {
        // TODO(refresh-story): Refresh token persistence and rotation are owned by
        // .squad/stories/auth/refresh-access-token/intake.md. This method generates
        // the token value but does not persist it; the refresh endpoint will handle storage.
        var randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        return Convert.ToBase64String(randomBytes);
    }
}
