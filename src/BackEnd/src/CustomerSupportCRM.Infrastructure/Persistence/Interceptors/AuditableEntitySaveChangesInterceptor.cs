using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CustomerSupportCRM.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Populates <see cref="BaseEntity"/> audit fields on every save and enforces
/// soft delete: any entity marked <see cref="EntityState.Deleted"/> (e.g. via a
/// stray <c>DbSet.Remove</c> call bypassing the repository) is converted to a
/// <see cref="EntityState.Modified"/> update with <see cref="BaseEntity.IsDeleted"/>
/// set instead, so no CRM data is ever physically deleted through normal use.
/// Uses <see cref="ICurrentUserService"/> so Domain/Infrastructure never touch
/// <c>HttpContext</c> directly; <c>CreatedBy</c>/<c>UpdatedBy</c> are left
/// <c>null</c> for unauthenticated or background operations. All timestamps are UTC.
/// </summary>
public sealed class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService? _currentUserService;

    public AuditableEntitySaveChangesInterceptor(ICurrentUserService? currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        ApplyAuditRulesAndSoftDelete(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAuditRulesAndSoftDelete(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAuditRulesAndSoftDelete(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var userId = _currentUserService?.UserId?.ToString();
        var utcNow = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = utcNow;
                    entry.Entity.CreatedBy = userId;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = utcNow;
                    entry.Entity.UpdatedBy = userId;
                    break;

                case EntityState.Deleted:
                    ConvertToSoftDelete(entry, utcNow, userId);
                    break;
            }
        }
    }

    private static void ConvertToSoftDelete(EntityEntry<BaseEntity> entry, DateTime utcNow, string? userId)
    {
        entry.State = EntityState.Modified;
        entry.Entity.IsDeleted = true;
        entry.Entity.UpdatedAt = utcNow;
        entry.Entity.UpdatedBy = userId;
    }
}
