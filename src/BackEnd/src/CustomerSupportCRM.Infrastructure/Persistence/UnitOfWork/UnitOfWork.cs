using System.Collections.Concurrent;
using CustomerSupportCRM.Domain.Common;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using CustomerSupportCRM.Infrastructure.Persistence.Context;
using CustomerSupportCRM.Infrastructure.Persistence.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CustomerSupportCRM.Infrastructure.Persistence.UnitOfWork;

/// <summary>
/// EF Core implementation of <see cref="IUnitOfWork"/>. Caches one
/// <see cref="GenericRepository{T}"/> per entity type for the lifetime of the
/// (scoped) unit of work instance and commits everything through a single
/// <see cref="ApplicationDbContext.SaveChangesAsync(CancellationToken)"/> call.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly ConcurrentDictionary<Type, object> _repositories = new();

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<T> Repository<T>() where T : BaseEntity
    {
        var repository = _repositories.GetOrAdd(
            typeof(T),
            _ => new GenericRepository<T>(_context));

        return (IGenericRepository<T>)repository;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            // Bug fix: a handler that pre-checks uniqueness (ExistsAsync) then
            // inserts (e.g. QuickReplyTemplate name-per-owner,
            // CustomerFeedback one-per-ticket-per-customer) still has a
            // narrow TOCTOU race window; the unique index is the real
            // safety net and always prevented the duplicate row, but an EF
            // DbUpdateException from that race was previously unhandled and
            // fell through to a generic 500. Centralized here (Infrastructure,
            // where EF/SQL types are actually available - Application/Domain
            // never reference them) so it covers every current and future
            // unique index in the schema, not just the ones we happened to
            // special-case per handler.
            throw new ConflictException(
                "The request could not be completed because it conflicts with an existing record.");
        }
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex) =>
        ex.InnerException is SqlException { Number: 2601 or 2627 };
}
