namespace CustomerSupportCRM.Domain.Exceptions;

/// <summary>
/// Thrown when an operation requires an authenticated principal and none is
/// present. Distinct from <see cref="System.UnauthorizedAccessException"/> so the
/// API's exception handler can map it deliberately, without accidentally catching
/// unrelated .NET framework/runtime access violations. Maps to HTTP 401.
/// </summary>
public sealed class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message = "Authentication is required to perform this action.")
        : base(message)
    {
    }
}
