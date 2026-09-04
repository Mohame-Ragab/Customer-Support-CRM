using System.Security.Cryptography;
using System.Text;
using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CustomerSupportCRM.Infrastructure.Identity.Services;

public sealed class RefreshTokenService : IRefreshTokenService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<RefreshTokenService> _logger;

    public RefreshTokenService(
        ApplicationDbContext context,
        IOptions<JwtSettings> jwtSettings,
        ILogger<RefreshTokenService> logger)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    public async Task<(string RawToken, DateTime ExpiresAt)> IssueAsync(
        string userId, string? ip, CancellationToken ct)
    {
        if (!Guid.TryParse(userId, out var userIdGuid))
        {
            throw new ArgumentException("Invalid user ID format", nameof(userId));
        }

        var rawToken = GenerateToken();
        var tokenHash = HashToken(rawToken);
        var expiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays);

        var refreshToken = new RefreshToken
        {
            UserId = userIdGuid,
            TokenHash = tokenHash,
            ExpiresAt = expiresAt,
            CreatedByIp = ip,
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Refresh token issued for user {UserId}", userId);
        return (rawToken, expiresAt);
    }

    public async Task<(string UserId, string NewRawToken, DateTime NewExpiresAt)> RotateAsync(
        string presentedRawToken, string? ip, CancellationToken ct)
    {
        var presentedHash = HashToken(presentedRawToken);
        var oldToken = await _context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TokenHash == presentedHash, ct);

        if (oldToken == null)
        {
            _logger.LogWarning("Refresh token rotation failed: unknown token");
            throw new UnauthorizedException("Invalid refresh token.");
        }

        // Replay detection: if the token was already rotated, revoke the entire descendant chain
        if (!oldToken.IsActive && oldToken.ReplacedByTokenHash != null)
        {
            _logger.LogWarning("Refresh token replay detected for user {UserId}", oldToken.UserId);
            await RevokeDescendantChainAsync(oldToken.UserId, ip, ct);
            throw new UnauthorizedException("Invalid refresh token.");
        }

        if (oldToken.IsExpired || oldToken.IsRevoked)
        {
            _logger.LogWarning("Refresh token rotation failed: token expired or revoked for user {UserId}", oldToken.UserId);
            throw new UnauthorizedException("Invalid refresh token.");
        }

        // Mint new token
        var newRawToken = GenerateToken();
        var newTokenHash = HashToken(newRawToken);
        var newExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays);

        using var transaction = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            // Mark old token as replaced
            var oldTokenTracked = await _context.RefreshTokens.FirstAsync(x => x.Id == oldToken.Id, ct);
            oldTokenTracked.RevokedAt = DateTime.UtcNow;
            oldTokenTracked.RevokedByIp = ip;
            oldTokenTracked.ReplacedByTokenHash = newTokenHash;

            // Create new token
            var newToken = new RefreshToken
            {
                UserId = oldToken.UserId,
                TokenHash = newTokenHash,
                ExpiresAt = newExpiresAt,
                CreatedByIp = ip,
            };

            _context.RefreshTokens.Update(oldTokenTracked);
            _context.RefreshTokens.Add(newToken);
            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            _logger.LogInformation("Refresh token rotated for user {UserId}", oldToken.UserId);
            return (oldToken.UserId.ToString(), newRawToken, newExpiresAt);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task RevokeAsync(string presentedRawToken, string? ip, CancellationToken ct)
    {
        var presentedHash = HashToken(presentedRawToken);
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == presentedHash, ct);

        if (token?.IsActive == true)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = ip;
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync(ct);
            _logger.LogInformation("Refresh token revoked for user {UserId}", token.UserId);
        }
    }

    private async Task RevokeDescendantChainAsync(Guid userId, string? ip, CancellationToken ct)
    {
        var tokensToRevoke = await _context.RefreshTokens
            .Where(x => x.UserId == userId && x.IsActive)
            .ToListAsync(ct);

        foreach (var token in tokensToRevoke)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = ip;
        }

        if (tokensToRevoke.Any())
        {
            _context.RefreshTokens.UpdateRange(tokensToRevoke);
            await _context.SaveChangesAsync(ct);
            _logger.LogInformation("Revoked {Count} refresh tokens for user {UserId} due to replay", tokensToRevoke.Count, userId);
        }
    }

    private static string GenerateToken()
    {
        var randomBytes = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return WebEncoders.Base64UrlEncode(randomBytes);
    }

    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash);
    }
}
