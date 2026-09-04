using CustomerSupportCRM.Application.Features.Customers.Attachments.Commands.UploadCustomerAttachment;
using CustomerSupportCRM.Application.Features.Customers.Attachments.Dtos;
using CustomerSupportCRM.Application.Features.Customers.Attachments.Queries;
using CustomerSupportCRM.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerSupportCRM.API.Controllers;

/// <summary>
/// Staff-facing file attachments on a customer (customers/manage-customer-attachments).
/// Upload, list, and download only - deletion is explicitly out of scope.
/// </summary>
[ApiController]
[Route("api/customers/{customerId:guid}/attachments")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Supervisor},{Roles.Agent}")]
public sealed class CustomerAttachmentsController : BaseApiController
{
    private readonly IMediator _mediator;

    public CustomerAttachmentsController(IMediator mediator) => _mediator = mediator;

    /// <summary>POST - multipart/form-data upload, field name "file".</summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(20 * 1024 * 1024)] // headroom above MaxFileSizeBytes; the handler enforces the exact limit
    [ProducesResponseType(typeof(CustomerAttachmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerAttachmentDto>> Upload([FromForm] UploadAttachmentRequest request, CancellationToken ct)
    {
        await using var stream = request.File.OpenReadStream();
        var command = new UploadCustomerAttachmentCommand(
            request.CustomerId, stream, request.File.FileName, request.File.ContentType, request.File.Length);

        var attachment = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(List), new { request.CustomerId }, attachment);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerAttachmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<CustomerAttachmentDto>>> List(Guid customerId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetCustomerAttachmentsQuery(customerId), ct));

    [HttpGet("{attachmentId:guid}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download(Guid customerId, Guid attachmentId, CancellationToken ct)
    {
        var result = await _mediator.Send(new DownloadCustomerAttachmentQuery(customerId, attachmentId), ct);
        return File(result.Content, result.ContentType, result.FileName);
    }
}
