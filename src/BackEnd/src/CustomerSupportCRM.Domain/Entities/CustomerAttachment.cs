using CustomerSupportCRM.Domain.Common;

namespace CustomerSupportCRM.Domain.Entities;

/// <summary>
/// A file uploaded by staff and associated with a single Customer
/// (customers/manage-customer-attachments). Metadata only lives here; the raw
/// bytes are persisted through IFileStorageService. Deletion is out of scope.
/// </summary>
public class CustomerAttachment : BaseEntity
{
    public Guid CustomerId { get; set; }

    /// <summary>Original filename supplied by the client (sanitized).</summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>MIME content type as reported at upload time.</summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>Size in bytes.</summary>
    public long SizeBytes { get; set; }

    /// <summary>Opaque key returned by IFileStorageService.SaveAsync (e.g. relative path).</summary>
    public string StorageKey { get; set; } = string.Empty;
}
