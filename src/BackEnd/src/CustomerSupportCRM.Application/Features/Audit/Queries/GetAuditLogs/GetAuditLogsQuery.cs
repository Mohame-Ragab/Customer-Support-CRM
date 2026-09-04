using CustomerSupportCRM.Application.Common.Models;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Audit.Queries.GetAuditLogs;

public sealed record GetAuditLogsQuery(int Page = 1, int PageSize = 50) : IRequest<PagedResult<AuditLogEntryDto>>;
