using CustomerSupportCRM.Application.Features.Branding;
using CustomerSupportCRM.Application.Features.Branding.Dtos;
using CustomerSupportCRM.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupportCRM.API.Controllers;

/// <summary>
/// Tenant branding: primary/secondary colors + optional logo (platform/custom-branding).
/// The public endpoint is anonymous so the login page (pre-authentication)
/// can render the tenant's branding; the role check on PUT is the real
/// security boundary - the frontend's role-based UI hiding is UX only.
/// </summary>
[Route("api/branding")]
public sealed class BrandingController : BaseApiController
{
    private readonly IBrandingService _brandingService;

    public BrandingController(IBrandingService brandingService) => _brandingService = brandingService;

    /// <summary>Public branding fetch - used by the login page and any anonymous view before authentication.</summary>
    [AllowAnonymous]
    [HttpGet("public")]
    [ProducesResponseType(typeof(BrandingDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BrandingDto>> GetPublic(CancellationToken ct)
    {
        Response.Headers.CacheControl = "public, max-age=60";
        return Ok(await _brandingService.GetAsync(ct));
    }

    /// <summary>Authenticated fetch, used by the admin branding form to preload current values.</summary>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(BrandingDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BrandingDto>> Get(CancellationToken ct)
        => Ok(await _brandingService.GetAsync(ct));

    [Authorize(Roles = Roles.Admin)]
    [HttpPut]
    [ProducesResponseType(typeof(BrandingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<BrandingDto>> Update([FromBody] UpdateBrandingRequest request, CancellationToken ct)
        => Ok(await _brandingService.UpdateAsync(request, ct));
}
