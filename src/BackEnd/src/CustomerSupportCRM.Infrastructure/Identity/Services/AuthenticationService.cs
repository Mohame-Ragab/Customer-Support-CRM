using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.Auth.Login;
using CustomerSupportCRM.Domain.Constants;
using Microsoft.AspNetCore.Identity;

namespace CustomerSupportCRM.Infrastructure.Identity.Services;

public sealed class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthenticationService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenGenerator tokenGenerator,
        IRefreshTokenService refreshTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenGenerator = tokenGenerator;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken ct)
    {
        // Step 1: Find user by email
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result<LoginResponse>.FailureWithKey("Auth_InvalidCredentials");
        }

        // Step 2: Check password and handle lockout
        var signIn = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (signIn.IsLockedOut)
        {
            return Result<LoginResponse>.FailureWithKey("Auth_AccountLocked");
        }

        if (!signIn.Succeeded)
        {
            return Result<LoginResponse>.FailureWithKey("Auth_InvalidCredentials");
        }

        // Step 3: Get user roles
        var roles = await _userManager.GetRolesAsync(user);

        // Step 4: Check if Customer role requires email verification
        if (roles.Contains(Roles.Customer) && !user.EmailConfirmed)
        {
            return Result<LoginResponse>.FailureWithKey("Auth_EmailNotVerified");
        }

        // Step 5: Generate tokens.
        // Bug fix: this previously called _tokenGenerator.GenerateRefreshToken()
        // directly, which only mints a random string and - per its own TODO
        // comment - never persists it ("the refresh endpoint will handle
        // storage"). Nothing ever called that storage step at login, so the
        // token returned here had no matching row in RefreshTokens; the very
        // first POST /refresh after any login always failed with 401 "Invalid
        // refresh token". Fixed by issuing (and persisting) the refresh token
        // through IRefreshTokenService instead - the same service /refresh
        // already uses to rotate it.
        var displayName = user.UserName ?? user.Email ?? string.Empty;
        var (accessToken, expiresAtUtc) = _tokenGenerator.GenerateAccessToken(user.Id, user.Email!, displayName, (IReadOnlyCollection<string>)roles);
        var (refreshToken, refreshExpiresAt) = await _refreshTokenService.IssueAsync(user.Id.ToString(), ipAddress, ct);

        // Step 6: Return response
        var response = new LoginResponse(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            ExpiresAtUtc: expiresAtUtc,
            TokenType: "Bearer",
            UserId: user.Id,
            Email: user.Email!,
            DisplayName: displayName,
            Roles: (IReadOnlyList<string>)roles,
            RefreshTokenExpiresAtUtc: refreshExpiresAt);

        return Result<LoginResponse>.Success(response);
    }
}
