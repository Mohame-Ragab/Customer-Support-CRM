using CustomerSupportCRM.Application.Features.RolePermissions.Commands;
using CustomerSupportCRM.Application.Features.RolePermissions.Dtos;
using CustomerSupportCRM.Application.Features.RolePermissions.Queries;
using CustomerSupportCRM.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupportCRM.API.Controllers.Admin;

/// <summary>
/// Admin catalog listing and per-role permission grants
/// (security-admin/manage-role-permissions). Class-gated to Roles.Admin today;
/// a follow-up story will migrate this to
/// <c>[Authorize(Policy = Permissions.RolePermissions.Manage)]</c> once the
/// catalog is stable.
/// </summary>
[Authorize(Roles = Roles.Admin)]
[Route("api/admin/role-permissions")]
public sealed class RolePermissionsController : BaseApiController
{
    private readonly IMediator _mediator;

    public RolePermissionsController(IMediator mediator) => _mediator = mediator;

    /// <summary>The full permission catalog the system recognizes.</summary>
    [HttpGet("catalog")]
    [ProducesResponseType(typeof(IReadOnlyCollection<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<string>>> GetCatalog(CancellationToken ct)
        => Ok(await _mediator.Send(new GetPermissionCatalogQuery(), ct));

    /// <summary>Permissions currently granted to one role.</summary>
    [HttpGet("{roleId:guid}")]
    [ProducesResponseType(typeof(RolePermissionsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RolePermissionsDto>> GetRolePermissions(Guid roleId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetRolePermissionsQuery(roleId), ct));

    /// <summary>Grants a permission to a role.</summary>
    [HttpPost("{roleId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AssignPermission(Guid roleId, [FromBody] AssignPermissionRequest body, CancellationToken ct)
    {
        await _mediator.Send(new AssignPermissionCommand(roleId, body.PermissionName), ct);
        return NoContent();
    }

    /// <summary>Removes a permission from a role. Idempotent.</summary>
    [HttpDelete("{roleId:guid}/{permissionName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RemovePermission(Guid roleId, string permissionName, CancellationToken ct)
    {
        await _mediator.Send(new RemovePermissionCommand(roleId, permissionName), ct);
        return NoContent();
    }
}
