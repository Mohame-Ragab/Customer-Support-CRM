using CustomerSupportCRM.Application.Features.Customers.Notes.Commands.CreateCustomerNote;
using CustomerSupportCRM.Application.Features.Customers.Notes.Dtos;
using CustomerSupportCRM.Application.Features.Customers.Notes.Queries;
using CustomerSupportCRM.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupportCRM.API.Controllers;

/// <summary>Append-only internal staff notes on a customer (customers/manage-customer-notes). Edit/delete out of scope.</summary>
[ApiController]
[Route("api/customers/{customerId:guid}/notes")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Supervisor},{Roles.Manager},{Roles.Agent}")]
public sealed class CustomerNotesController : BaseApiController
{
    private readonly IMediator _mediator;

    public CustomerNotesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerNoteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<CustomerNoteDto>>> List(Guid customerId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetCustomerNotesQuery(customerId), ct));

    [HttpPost]
    [ProducesResponseType(typeof(CustomerNoteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerNoteDto>> Add(
        Guid customerId, [FromBody] CreateCustomerNoteRequest request, CancellationToken ct)
    {
        var note = await _mediator.Send(new CreateCustomerNoteCommand(customerId, request.Content), ct);
        return CreatedAtAction(nameof(List), new { customerId }, note);
    }
}
