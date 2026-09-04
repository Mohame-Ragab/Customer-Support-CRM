using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Common.Models;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Audit.Queries.GetAuditLogs;

public sealed class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, PagedResult<AuditLogEntryDto>>
{
    private readonly IAuditLogService _auditLogService;

    public GetAuditLogsQueryHandler(IAuditLogService auditLogService) => _auditLogService = auditLogService;

    public Task<PagedResult<AuditLogEntryDto>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
        => _auditLogService.GetAuditLogsAsync(request.Page, request.PageSize, cancellationToken);
}
