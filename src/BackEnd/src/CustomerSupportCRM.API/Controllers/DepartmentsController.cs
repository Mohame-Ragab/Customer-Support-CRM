using CustomerSupportCRM.Application.Features.Departments;
using CustomerSupportCRM.Application.Features.Departments.DTOs;
using CustomerSupportCRM.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupportCRM.API.Controllers;

/// <summary>
/// Department CRUD (platform/multi-department-support). Reads are open to any
/// authenticated user (needed by downstream department pickers); writes are
/// Administrator-only. Deletion is a soft delete (IsActive = false).
/// </summary>
[Authorize]
[Route("api/departments")]
public sealed class DepartmentsController : BaseApiController
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService) => _departmentService = departmentService;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<DepartmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetList(
        [FromQuery] bool includeInactive, CancellationToken ct)
        => Ok(await _departmentService.GetListAsync(includeInactive, ct));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DepartmentDto>> GetById(
        Guid id, [FromQuery] bool includeInactive, CancellationToken ct)
        => Ok(await _departmentService.GetByIdAsync(id, includeInactive, ct));

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<DepartmentDto>> Create(
        [FromBody] CreateDepartmentRequest request, CancellationToken ct)
    {
        var department = await _departmentService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<DepartmentDto>> Update(
        Guid id, [FromBody] UpdateDepartmentRequest request, CancellationToken ct)
        => Ok(await _departmentService.UpdateAsync(id, request, ct));

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _departmentService.DeleteAsync(id, ct);
        return NoContent();
    }
}
