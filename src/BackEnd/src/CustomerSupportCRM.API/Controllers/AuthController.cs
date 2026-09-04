using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Features.Auth.Login;
using CustomerSupportCRM.Application.Features.Auth.Commands.RegisterCustomer;
using CustomerSupportCRM.Application.Features.Auth.Dtos;
using CustomerSupportCRM.Application.Features.Auth.Refresh;
using CustomerSupportCRM.Application.Features.Users.ChangePassword;
using CustomerSupportCRM.Application.Features.Users.Commands.ForgotPassword;
using CustomerSupportCRM.Application.Features.Users.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CustomerSupportCRM.API.Controllers;

/// <summary>
/// Authentication endpoints: login, logout, token refresh, password reset, etc.
/// All endpoints use the /api/v1/auth/* prefix per CRIT-02 standardization.
/// </summary>
[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : BaseApiController
{
    private readonly IAuthenticationService _authService;
    private readonly IIdentityService _identityService;
    private readonly IRegistrationService _registrationService;
    private readonly IEmailVerificationService _emailVerificationService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<Resources.SharedResource> _localizer;
    private readonly IMediator _mediator;

    public AuthController(
        IAuthenticationService authService,
        IIdentityService identityService,
        IRegistrationService registrationService,
        IEmailVerificationService emailVerificationService,
        IRefreshTokenService refreshTokenService,
        IJwtTokenGenerator jwtTokenGenerator,
        ICurrentUserService currentUserService,
        IStringLocalizer<Resources.SharedResource> localizer,
        IMediator mediator)
    {
        _authService = authService;
        _identityService = identityService;
        _registrationService = registrationService;
        _emailVerificationService = emailVerificationService;
        _refreshTokenService = refreshTokenService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _currentUserService = currentUserService;
        _localizer = localizer;
        _mediator = mediator;
    }

    /// <summary>
    /// POST /api/v1/auth/login
    ///
    /// Authenticates a user with email and password. On success, returns access and refresh tokens.
    /// On failure, returns a localized error message with a stable error code.
    /// </summary>
    /// <remarks>
    /// - Unknown email and wrong password both return Auth_InvalidCredentials (indistinguishable).
    /// - Customer accounts with EmailConfirmed = false are blocked with Auth_EmailNotVerified.
    /// - Accounts locked due to too many failed attempts return Auth_AccountLocked.
    /// </remarks>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.LoginAsync(request, ip, ct);

        if (result.Succeeded)
        {
            SetRefreshTokenCookie(result.Value!.RefreshToken, result.Value.RefreshTokenExpiresAtUtc);
            return Ok(result.Value);
        }

        var errorKey = result.ErrorKey ?? "UnexpectedError";
        var localizedMessage = _localizer[errorKey].Value;

        return Unauthorized(new
        {
            message = localizedMessage,
            code = errorKey
        });
    }

    /// <summary>
    /// POST /api/v1/auth/change-password
    ///
    /// Allows an authenticated user to change their password by providing their current
    /// password and a new password that must satisfy the password policy.
    /// </summary>
    /// <remarks>
    /// - Requires authentication ([Authorize]).
    /// - Wrong current password returns 401 Unauthorized.
    /// - Password policy violation or new password equals current returns 400 Bad Request.
    /// - Success returns 204 No Content.
    /// </remarks>
    [Authorize]
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordCommand command,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var outcome = await _identityService.ChangePasswordAsync(
            userId.Value.ToString(),
            command.CurrentPassword,
            command.NewPassword,
            cancellationToken);

        if (!outcome.Succeeded)
        {
            // If PasswordMismatch, return 401
            if (outcome.Errors.Contains("PasswordMismatch"))
            {
                return Unauthorized(new
                {
                    message = _localizer["current_password_incorrect"].Value,
                    code = "current_password_incorrect"
                });
            }

            // Otherwise return 400 with validation errors
            var problemDetails = new ValidationProblemDetails
            {
                Title = _localizer["ValidationFailed"].Value,
                Status = StatusCodes.Status400BadRequest,
                Errors = new Dictionary<string, string[]>
                {
                    ["newPassword"] = new[] { _localizer["password_policy_violation"].Value }
                }
            };

            return BadRequest(problemDetails);
        }

        return NoContent();
    }

    /// <summary>
    /// POST /api/v1/auth/register
    ///
    /// Allows a prospective customer to self-register with email and password.
    /// Creates an account in the Customer role with EmailConfirmed = false.
    /// Queues email verification but does not send it in this story.
    /// </summary>
    /// <remarks>
    /// - Requires no authentication ([AllowAnonymous]).
    /// - Duplicate email returns 400 Bad Request with a generic message that does not leak account existence.
    /// - Success returns 201 Created.
    /// </remarks>
    /// <summary>
    /// POST /api/v1/auth/register
    ///
    /// Allows a prospective customer to self-register with email and password.
    /// Creates an account in the Customer role with EmailConfirmed = false.
    /// Queues email verification but does not send it in this story.
    /// </summary>
    /// <remarks>
    /// - Requires no authentication ([AllowAnonymous]).
    /// - Duplicate email returns 400 Bad Request with a generic message that does not leak account existence.
    /// - Success returns 201 Created.
    /// </remarks>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RegisterCustomerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCustomerCommand command,
        CancellationToken cancellationToken)
    {
        var (success, response, errorKey) = await _registrationService.RegisterAsync(command, cancellationToken);

        if (!success)
        {
            var localizedMessage = _localizer[errorKey ?? "UnexpectedError"].Value;
            var problemDetails = new ProblemDetails
            {
                Title = _localizer["ValidationFailed"].Value,
                Status = StatusCodes.Status400BadRequest,
                Detail = localizedMessage,
            };
            return BadRequest(problemDetails);
        }

        return Created(string.Empty, response);
    }

    [HttpPost("verify-email")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request, CancellationToken ct)
    {
        var result = await _emailVerificationService.ConfirmAsync(request.UserId, request.Token, ct);
        if (!result.Succeeded)
        {
            return BadRequest(new { code = "Auth.Verification.InvalidOrExpired", message = _localizer["Auth.Verification.InvalidOrExpired"].Value });
        }
        return Ok(new { message = _localizer["Auth.Verification.Success"].Value });
    }

    [HttpPost("resend-verification")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationRequest request, CancellationToken ct)
    {
        await _emailVerificationService.ResendAsync(request.Email, ct);
        return Ok(new { message = _localizer["Auth.Verification.Success"].Value });
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ForgotPasswordResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ForgotPasswordResponse>> ForgotPassword(
        [FromBody] ForgotPasswordCommand command,
        CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// Ends the caller's session on this client. Idempotent: returns 204 whether
    /// or not the caller presents a valid token.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        // Bug fix: this previously did nothing but clear client-side state - the
        // refresh token cookie stayed valid server-side, so a captured/replayed
        // cookie could still mint new access tokens after "logout". Now revokes
        // the presented token (best-effort - logout must still succeed even if
        // the token is already invalid/missing) and clears the cookie.
        if (Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken) && !string.IsNullOrEmpty(refreshToken))
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            try
            {
                await _refreshTokenService.RevokeAsync(refreshToken, ip, ct);
            }
            catch
            {
                // Best-effort - an already-invalid token must not block logout.
            }
        }

        ClearRefreshTokenCookie();
        return NoContent();
    }

    /// <summary>
    /// Refreshes an expired access token using the refresh token. Returns a new
    /// access token and (via an HttpOnly cookie) a rotated refresh token.
    /// Refresh tokens are single-use and cannot be replayed after rotation.
    /// </summary>
    /// <remarks>
    /// The refresh token itself travels as an HttpOnly, Secure, SameSite=Lax
    /// cookie (never readable from JavaScript) - set here and by /login. The
    /// request body's RefreshToken is optional and kept only for API
    /// compatibility; when omitted, the cookie is used. This lets the frontend
    /// silently restore a session after a page reload by calling this endpoint
    /// with no body - the browser attaches the cookie automatically.
    /// </remarks>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RefreshResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshRequest? request,
        [FromServices] Microsoft.AspNetCore.Identity.UserManager<Infrastructure.Identity.ApplicationUser> userManager,
        CancellationToken ct)
    {
        var presentedToken = request?.RefreshToken;
        if (string.IsNullOrEmpty(presentedToken))
        {
            Request.Cookies.TryGetValue(RefreshTokenCookieName, out presentedToken);
        }

        if (string.IsNullOrEmpty(presentedToken))
        {
            return Unauthorized();
        }

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        string userId;
        string newRefresh;
        DateTime refreshExpiresAt;
        try
        {
            (userId, newRefresh, refreshExpiresAt) = await _refreshTokenService.RotateAsync(presentedToken, ip, ct);
        }
        catch (Domain.Exceptions.UnauthorizedException)
        {
            ClearRefreshTokenCookie();
            return Unauthorized();
        }

        // Fetch user to generate new access token with their details
        if (!Guid.TryParse(userId, out var userIdGuid))
        {
            return Unauthorized();
        }

        var user = await userManager.FindByIdAsync(userIdGuid.ToString());
        if (user == null)
        {
            return Unauthorized();
        }

        var roles = await userManager.GetRolesAsync(user);
        var (access, accessExpiresAt) = _jwtTokenGenerator.GenerateAccessToken(
            userIdGuid, user.Email ?? string.Empty, user.UserName ?? user.Email ?? string.Empty, (IReadOnlyCollection<string>)roles);

        SetRefreshTokenCookie(newRefresh, refreshExpiresAt);
        return Ok(new RefreshResponse(access, accessExpiresAt, newRefresh, refreshExpiresAt));
    }

    private const string RefreshTokenCookieName = "refreshToken";

    private void SetRefreshTokenCookie(string token, DateTime expiresAtUtc)
    {
        Response.Cookies.Append(RefreshTokenCookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = !HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment(),
            SameSite = SameSiteMode.Lax,
            Path = "/api/v1/auth",
            Expires = expiresAtUtc,
        });
    }

    private void ClearRefreshTokenCookie()
    {
        Response.Cookies.Delete(RefreshTokenCookieName, new CookieOptions { Path = "/api/v1/auth" });
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        if (!result.Succeeded)
        {
            var errorKey = result.Errors.FirstOrDefault() ?? "UnexpectedError";
            var localizedMessage = _localizer[errorKey].Value;
            return BadRequest(new { message = localizedMessage, code = errorKey });
        }
        return NoContent();
    }
}
