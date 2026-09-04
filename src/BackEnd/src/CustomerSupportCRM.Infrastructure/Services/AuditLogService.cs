using System.Text.Json;
using CustomerSupportCRM.Application.Common.Auditing;
using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.Audit.Queries.GetAuditLogs;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CustomerSupportCRM.Infrastructure.Services;

/// <summary>
/// Writer/reader for the security-admin audit log (security-admin/view-audit-logs).
/// Writes are deliberately not enrolled in the calling handler's transaction -
/// a separate <c>SaveChangesAsync</c> means a failed audit write never loses
/// the caller's own data. Failures are swallowed (logged) in Production so a
/// broken audit pipe can't take down the feature it's auditing; rethrown in
/// Development so regressions are caught early.
/// </summary>
public sealed class AuditLogService : IAuditLogService
{
    private const int MaxMetadataJsonLength = 8 * 1024; // 8 KB

    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<AuditLogService> _logger;

    public AuditLogService(
        ApplicationDbContext context,
        ICurrentUserService currentUserService,
        IHostEnvironment environment,
        ILogger<AuditLogService> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _environment = environment;
        _logger = logger;
    }

    public async Task WriteAsync(
        string action,
        string? entityType = null,
        string? entityId = null,
        string? summary = null,
        object? metadata = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var redacted = SensitiveFieldRedactor.Redact(metadata);
            var metadataJson = redacted == null
                ? null
                : JsonSerializer.Serialize(redacted, new JsonSerializerOptions { WriteIndented = false });

            if (metadataJson != null && metadataJson.Length > MaxMetadataJsonLength)
            {
                metadataJson = metadataJson[..MaxMetadataJsonLength] + "...[truncated]";
            }

            var entry = AuditLogEntry.Create(
                action,
                _currentUserService.UserId?.ToString(),
                _currentUserService.UserName,
                entityType,
                entityId,
                summary,
                metadataJson);

            _context.AuditLogs.Add(entry);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to write audit log entry for action {Action}", action);

            if (_environment.IsDevelopment())
            {
                throw;
            }
        }
    }

    public async Task<PagedResult<AuditLogEntryDto>> GetAuditLogsAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var query = _context.AuditLogs.AsNoTracking().OrderByDescending(e => e.TimestampUtc);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new AuditLogEntryDto(
                e.Id, e.TimestampUtc, e.PerformedByUserId, e.PerformedByUserName,
                e.Action, e.EntityType, e.EntityId, e.Summary, e.MetadataJson))
            .ToListAsync(ct);

        return new PagedResult<AuditLogEntryDto>(items, page, pageSize, totalCount);
    }
}
