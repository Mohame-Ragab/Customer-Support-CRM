using CustomerSupportCRM.Application.Features.RolePermissions.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.RolePermissions.Queries;

public sealed class GetRolePermissionsQueryHandler : IRequestHandler<GetRolePermissionsQuery, RolePermissionsDto>
{
    private readonly IRolePermissionAdminService _service;

    public GetRolePermissionsQueryHandler(IRolePermissionAdminService service) => _service = service;

    public Task<RolePermissionsDto> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
        => _service.GetRolePermissionsAsync(request.RoleId, cancellationToken);
}
