using MediatR;

namespace CustomerSupportCRM.Application.Features.RolePermissions.Queries;

public sealed class GetPermissionCatalogQueryHandler
    : IRequestHandler<GetPermissionCatalogQuery, IReadOnlyCollection<string>>
{
    private readonly IRolePermissionAdminService _service;

    public GetPermissionCatalogQueryHandler(IRolePermissionAdminService service) => _service = service;

    public Task<IReadOnlyCollection<string>> Handle(GetPermissionCatalogQuery request, CancellationToken cancellationToken)
        => Task.FromResult(_service.GetCatalog());
}
