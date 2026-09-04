using CustomerSupportCRM.API.Extensions;
using CustomerSupportCRM.Application.Features.Channels.WebForms;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CustomerSupportCRM.API.Controllers.Channels;

/// <summary>Public, unauthenticated ticket intake from an external web form (F03 web-forms-channel).</summary>
[ApiController]
[AllowAnonymous]
[Route("api/v1/channels/web-forms")]
public sealed class WebFormsController : BaseApiController
{
    private readonly IMediator _mediator;

    public WebFormsController(IMediator mediator) => _mediator = mediator;

    [HttpPost("submissions")]
    [EnableRateLimiting(RateLimitingExtensions.WebFormsPolicyName)]
    [ProducesResponseType(typeof(SubmitWebFormResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Submit([FromBody] SubmitWebFormRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(request, ct);
        return CreatedAtAction(nameof(Submit), new { id = result.TicketId }, result);
    }
}
