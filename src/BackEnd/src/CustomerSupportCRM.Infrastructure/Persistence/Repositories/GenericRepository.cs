using System.Linq.Expressions;
using CustomerSupportCRM.Domain.Common;
using CustomerSupportCRM.Domain.Interfaces;
using CustomerSupportCRM.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CustomerSupportCRM.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IGenericRepository{T}"/>. Read operations
/// use <c>AsNoTracking</c> since callers get entities back only to read them;
/// tracked lookups go through <see cref="GetByIdAsync"/> which callers use before
/// mutating. The global soft-delete query filter configured on
/// <see cref="ApplicationDbContext"/> applies automatically to every query here.
/// </summary>
public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbSet.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
        await _dbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public IQueryable<T> Query() => _dbSet.AsNoTracking();

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await _dbSet.AddAsync(entity, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default) =>
        await _dbSet.AddRangeAsync(entities, cancellationToken);

    public void Update(T entity) => _dbSet.Update(entity);

    /// <summary>Soft delete: marks the entity deleted; the audit interceptor also
    /// guards against a physical delete if this method is bypassed.</summary>
    public void Delete(T entity)
    {
        entity.IsDeleted = true;
        _dbSet.Update(entity);
    }

    public Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
        _dbSet.AnyAsync(predicate, cancellationToken);

    public Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default) =>
        predicate is null
            ? _dbSet.CountAsync(cancellationToken)
            : _dbSet.CountAsync(predicate, cancellationToken);
}
