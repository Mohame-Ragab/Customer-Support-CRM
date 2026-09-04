using MediatR;

namespace CustomerSupportCRM.Application.Features.RolePermissions.Commands;

public sealed record AssignPermissionCommand(Guid RoleId, string PermissionName) : IRequest;
