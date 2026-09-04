namespace CustomerSupportCRM.Domain.Exceptions;

/// <summary>Thrown when a requested entity does not exist. Maps to HTTP 404.</summary>
public sealed class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string entityName, object key)
        : base($"Entity \"{entityName}\" with key ({key}) was not found.")
    {
    }
}
