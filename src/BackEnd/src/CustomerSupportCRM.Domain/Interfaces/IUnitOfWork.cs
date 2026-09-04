using CustomerSupportCRM.Domain.Common;

namespace CustomerSupportCRM.Domain.Interfaces;

/// <summary>
/// Coordinates one or more repository operations within a single persistence
/// transaction. Kept deliberately generic and free of feature-specific
/// repository properties (e.g. no <c>Customers</c>/<c>Tickets</c> members) so it
/// does not need to change as CRM features are added.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Returns a generic repository for the given aggregate type. Infrastructure
    /// caches repositories per type within the unit of work's lifetime.
    /// </summary>
    IGenericRepository<T> Repository<T>() where T : BaseEntity;

    /// <summary>
    /// Persists all pending changes tracked by the underlying context in a single
    /// transaction and returns the number of affected records.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
