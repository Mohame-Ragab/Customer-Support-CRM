namespace CustomerSupportCRM.Application.Features.RolePermissions.Dtos;

/// <summary>POST /api/admin/role-permissions/{roleId} request body. The role id comes from the route, not this body.</summary>
public sealed record AssignPermissionRequest(string PermissionName);
