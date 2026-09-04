namespace CustomerSupportCRM.Domain.Exceptions;

/// <summary>
/// Base type for all exceptions raised from the Domain layer. Kept independent of
/// ASP.NET Core so it can be caught and translated to an HTTP response entirely
/// from the API's global exception handling middleware.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
