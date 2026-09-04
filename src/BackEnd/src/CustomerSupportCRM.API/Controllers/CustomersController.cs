using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.Customers.Commands.CreateCustomer;
using CustomerSupportCRM.Application.Features.Customers.Commands.UpdateCustomer;
using CustomerSupportCRM.Application.Features.Customers.Dtos;
using CustomerSupportCRM.Application.Features.Customers.InteractionHistory;
using CustomerSupportCRM.Application.Features.Customers.Queries.GetCustomerById;
using CustomerSupportCRM.Application.Features.Customers.Queries.GetCustomers;
using CustomerSupportCRM.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupportCRM.API.Controllers;

/// <summary>
/// Staff-facing customer profile CRUD (customers/create-customer,
/// view-customer, update-customer). All endpoints require a staff role -
/// Admin, Supervisor, Manager, or Agent. There is no "Customer" self-service
/// role access here; portal-authenticated customers are a separate concern
/// (see Customer entity's TODO(product) comment).
/// </summary>
[ApiController]
[Route("api/customers")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Supervisor},{Roles.Manager},{Roles.Agent}")]
public sealed class CustomersController : BaseApiController
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator) => _mediator = mediator;

    /// <summary>POST /api/customers - creates a customer profile. Duplicate email (case-insensitive) returns 409.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CustomerDto>> Create([FromBody] CreateCustomerCommand command, CancellationToken ct)
    {
        var created = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>GET /api/customers/{id} - a single customer profile.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetCustomerByIdQuery(id), ct));

    /// <summary>GET /api/customers?page&amp;pageSize - a paged list of customers, ordered by last name then first name.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<CustomerDto>>> GetList(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetCustomersQuery(page, pageSize), ct));

    /// <summary>PUT /api/customers/{id} - updates an existing customer profile.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CustomerDto>> Update(
        Guid id, [FromBody] UpdateCustomerRequest request, CancellationToken ct)
    {
        var command = new UpdateCustomerCommand(
            id, request.FirstName, request.LastName, request.Email,
            request.PhoneNumber, request.CompanyName, request.PreferredLanguage, request.Notes);

        return Ok(await _mediator.Send(command, ct));
    }

    /// <summary>GET /api/customers/{customerId}/interaction-history - aggregated interaction timeline (tickets today; channel communications reserved for F03).</summary>
    [HttpGet("{customerId:guid}/interaction-history")]
    [ProducesResponseType(typeof(CustomerInteractionHistoryResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerInteractionHistoryResult>> GetInteractionHistory(
        Guid customerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        => Ok(await _mediator.Send(new GetCustomerInteractionHistoryQuery(customerId, page, pageSize), ct));
}
