using MediatR;

namespace CustomerSupportCRM.Application.Features.RolePermissions.Commands;

public sealed class AssignPermissionCommandHandler : IRequestHandler<AssignPermissionCommand>
{
    private readonly IRolePermissionAdminService _service;

    public AssignPermissionCommandHandler(IRolePermissionAdminService service) => _service = service;

    public Task Handle(AssignPermissionCommand request, CancellationToken cancellationToken)
        => _service.AssignPermissionAsync(request.RoleId, request.PermissionName, cancellationToken);
}
