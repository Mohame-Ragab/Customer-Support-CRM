using MediatR;

namespace CustomerSupportCRM.Application.Features.RolePermissions.Commands;

public sealed record RemovePermissionCommand(Guid RoleId, string PermissionName) : IRequest;
