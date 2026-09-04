namespace CustomerSupportCRM.Application.Features.Auth.Refresh;

/// <summary>
/// Optional now (was a required non-nullable string): the refresh token
/// normally travels as an HttpOnly cookie (see AuthController.SetRefreshTokenCookie),
/// so a typical call posts an empty body. [ApiController]'s automatic
/// model-state validation rejects a null value for a non-nullable
/// constructor parameter with its own 400 shape *before* the controller
/// action ever runs - which broke the cookie fallback entirely (every
/// no-body /refresh call was rejected at the framework level). Kept as an
/// explicit parameter only for API-compatibility with a caller that still
/// wants to pass it in the body.
/// </summary>
public sealed record RefreshRequest(string? RefreshToken = null);
