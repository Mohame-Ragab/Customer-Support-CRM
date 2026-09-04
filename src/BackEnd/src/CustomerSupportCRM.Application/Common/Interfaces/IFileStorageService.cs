namespace CustomerSupportCRM.Application.Common.Interfaces;

/// <summary>File storage abstraction (customers/manage-customer-attachments). Local-disk today; swappable for cloud object storage without touching Application code.</summary>
public interface IFileStorageService
{
    /// <summary>Persists the stream and returns an opaque key to retrieve it later.</summary>
    Task<string> SaveAsync(
        Stream content,
        string suggestedFileName,
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>Opens a read stream for the given key. Throws NotFoundException if missing.</summary>
    Task<Stream> OpenReadAsync(string storageKey, CancellationToken cancellationToken = default);
}
