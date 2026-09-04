using System.Linq.Expressions;
using CustomerSupportCRM.Domain.Common;

namespace CustomerSupportCRM.Domain.Interfaces;

/// <summary>
/// Persistence-agnostic contract for basic CRUD access to a <see cref="BaseEntity"/>
/// aggregate. Implemented by Infrastructure using EF Core; Domain and Application
/// depend only on this abstraction, never on the concrete data access technology.
/// </summary>
public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Exposes a composable, read-only, non-tracked query for scenarios that need
    /// paging, projection, or ordering. Kept out of the write path.
    /// </summary>
    IQueryable<T> Query();

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    void Update(T entity);

    /// <summary>
    /// Marks the entity for removal. Implementations perform a soft delete
    /// (<c>IsDeleted = true</c>) rather than a physical delete.
    /// </summary>
    void Delete(T entity);

    Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default);
}
