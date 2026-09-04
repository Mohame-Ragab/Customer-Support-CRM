using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.Audit.Queries.GetAuditLogs;

namespace CustomerSupportCRM.Application.Common.Interfaces;

/// <summary>
/// Writer/reader contract for the security-admin audit log
/// (security-admin/view-audit-logs). Every F10 command handler that mutates
/// state (user create/update, role change, permission change, system
/// configuration change) calls <see cref="WriteAsync"/> after a successful
/// mutation. The implementation resolves the actor from
/// <see cref="ICurrentUserService"/>, stamps a UTC timestamp, and redacts
/// <c>metadata</c> before persisting it - callers never need to scrub secrets
/// themselves. Reads are routed through this interface too (rather than a
/// direct <c>ApplicationDbContext</c> reference) so <c>GetAuditLogsQueryHandler</c>
/// stays in Application without depending on Infrastructure/EF Core types.
/// </summary>
public interface IAuditLogService
{
    Task WriteAsync(
        string action,
        string? entityType = null,
        string? entityId = null,
        string? summary = null,
        object? metadata = null,
        CancellationToken cancellationToken = default);

    Task<PagedResult<AuditLogEntryDto>> GetAuditLogsAsync(int page, int pageSize, CancellationToken ct = default);
}
