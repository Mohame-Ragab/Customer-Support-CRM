using CustomerSupportCRM.Application.Features.Roles.Dtos;

namespace CustomerSupportCRM.Application.Features.Roles;

public interface IRoleAdminService
{
    Task<IReadOnlyList<RoleDto>> ListRolesAsync(CancellationToken ct);
    Task AssignRoleAsync(AssignRoleRequest request, CancellationToken ct);
}
