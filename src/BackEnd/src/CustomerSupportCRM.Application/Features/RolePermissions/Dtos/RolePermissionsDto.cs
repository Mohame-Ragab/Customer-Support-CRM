namespace CustomerSupportCRM.Application.Features.RolePermissions.Dtos;

public sealed record RolePermissionsDto(
    Guid RoleId,
    string RoleName,
    IReadOnlyCollection<string> Permissions);
