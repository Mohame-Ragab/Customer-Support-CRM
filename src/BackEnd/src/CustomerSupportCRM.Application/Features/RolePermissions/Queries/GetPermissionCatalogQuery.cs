using MediatR;

namespace CustomerSupportCRM.Application.Features.RolePermissions.Queries;

public sealed record GetPermissionCatalogQuery : IRequest<IReadOnlyCollection<string>>;
