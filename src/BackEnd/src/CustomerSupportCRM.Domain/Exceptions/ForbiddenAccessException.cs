namespace CustomerSupportCRM.Domain.Exceptions;

/// <summary>Thrown when an authenticated principal is not permitted to perform an operation. Maps to HTTP 403.</summary>
public sealed class ForbiddenAccessException : DomainException
{
    public ForbiddenAccessException(string message = "You do not have permission to perform this action.")
        : base(message)
    {
    }
}
