using CustomerSupportCRM.Application.Features.SystemConfiguration;
using CustomerSupportCRM.Application.Features.SystemConfiguration.Dtos;
using CustomerSupportCRM.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupportCRM.API.Controllers;

/// <summary>Admin-editable, database-backed operational settings (security-admin/manage-system-configuration). Distinct from the static appsettings.json-bound ApplicationSettings.</summary>
[Authorize(Roles = Roles.Admin)]
[Route("api/system-configuration")]
public sealed class SystemConfigurationController : BaseApiController
{
    private readonly ISystemConfigurationService _service;

    public SystemConfigurationController(ISystemConfigurationService service) => _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SystemSettingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SystemSettingDto>>> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    [HttpGet("{key}")]
    [ProducesResponseType(typeof(SystemSettingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SystemSettingDto>> GetByKey(string key, CancellationToken ct)
        => Ok(await _service.GetByKeyAsync(key, ct));

    [HttpPut("{key}")]
    [ProducesResponseType(typeof(SystemSettingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SystemSettingDto>> Update(string key, [FromBody] UpdateSystemSettingDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(key, dto, ct));
}
