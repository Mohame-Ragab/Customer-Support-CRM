namespace CustomerSupportCRM.Domain.Exceptions;

/// <summary>Thrown when an operation conflicts with the current state of a resource (e.g. a uniqueness rule). Maps to HTTP 409.</summary>
public sealed class ConflictException : DomainException
{
    public ConflictException(string message) : base(message)
    {
    }
}
