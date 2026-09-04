namespace CustomerSupportCRM.Application.Features.Roles.Dtos;

/// <summary>Assign a single role to a user. Replaces any existing role membership.</summary>
public sealed record AssignRoleRequest(Guid UserId, string RoleName);
