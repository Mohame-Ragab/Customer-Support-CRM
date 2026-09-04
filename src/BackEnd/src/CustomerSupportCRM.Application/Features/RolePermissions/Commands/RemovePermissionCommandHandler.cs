using MediatR;

namespace CustomerSupportCRM.Application.Features.RolePermissions.Commands;

public sealed class RemovePermissionCommandHandler : IRequestHandler<RemovePermissionCommand>
{
    private readonly IRolePermissionAdminService _service;

    public RemovePermissionCommandHandler(IRolePermissionAdminService service) => _service = service;

    public Task Handle(RemovePermissionCommand request, CancellationToken cancellationToken)
        => _service.RemovePermissionAsync(request.RoleId, request.PermissionName, cancellationToken);
}
