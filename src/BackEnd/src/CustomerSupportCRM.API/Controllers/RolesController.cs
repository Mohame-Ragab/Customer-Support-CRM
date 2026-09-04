using CustomerSupportCRM.Application.Features.Roles;
using CustomerSupportCRM.Application.Features.Roles.Dtos;
using CustomerSupportCRM.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupportCRM.API.Controllers;

/// <summary>Admin role listing and single-role assignment (security-admin/manage-roles). Does not create/rename/delete roles.</summary>
[Authorize(Roles = Roles.Admin)]
public sealed class RolesController : BaseApiController
{
    private readonly IRoleAdminService _roleAdminService;

    public RolesController(IRoleAdminService roleAdminService)
        => _roleAdminService = roleAdminService;

    /// <summary>List all roles defined in the system.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<RoleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> ListRoles(CancellationToken ct)
        => Ok(await _roleAdminService.ListRolesAsync(ct));

    /// <summary>Assign a role to an existing user, replacing any current role.</summary>
    [HttpPost("assignments")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request, CancellationToken ct)
    {
        await _roleAdminService.AssignRoleAsync(request, ct);
        return NoContent();
    }
}
