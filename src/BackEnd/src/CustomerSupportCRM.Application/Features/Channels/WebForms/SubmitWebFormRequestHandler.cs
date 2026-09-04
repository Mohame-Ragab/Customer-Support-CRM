using CustomerSupportCRM.Application.Features.Tickets.Commands.CreateTicket;
using CustomerSupportCRM.Application.Features.Tickets.CustomerResolution;
using CustomerSupportCRM.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CustomerSupportCRM.Application.Features.Channels.WebForms;

/// <summary>
/// Thin adapter: resolves/creates the submitting Customer by email (never
/// duplicating that lookup - see ICustomerResolver, shared with the live-chat
/// channel), then delegates ticket creation to the existing F02
/// CreateTicketCommand with Channel = WebForm.
/// </summary>
public sealed class SubmitWebFormRequestHandler : IRequestHandler<SubmitWebFormRequest, SubmitWebFormResponse>
{
    private readonly IMediator _mediator;
    private readonly ICustomerResolver _customerResolver;
    private readonly ILogger<SubmitWebFormRequestHandler> _logger;

    public SubmitWebFormRequestHandler(
        IMediator mediator, ICustomerResolver customerResolver, ILogger<SubmitWebFormRequestHandler> logger)
    {
        _mediator = mediator;
        _customerResolver = customerResolver;
        _logger = logger;
    }

    public async Task<SubmitWebFormResponse> Handle(SubmitWebFormRequest request, CancellationToken cancellationToken)
    {
        // Do not log the raw email/message at Information - PII. Debug only.
        _logger.LogDebug("Web form submission received for category {Category}", request.Category);

        var customerId = await _customerResolver.ResolveOrCreateByEmailAsync(
            request.SubmitterEmail, request.SubmitterName, cancellationToken);

        var subject = string.IsNullOrWhiteSpace(request.Category)
            ? request.Subject
            : $"[{request.Category}] {request.Subject}";

        var ticket = await _mediator.Send(
            new CreateTicketCommand(subject, request.Message, customerId, TicketChannel.WebForm),
            cancellationToken);

        // No separate tracking-code column: the ticket id itself (opaque "N"
        // format) is stable, unique, and sufficient for an anonymous
        // submitter to reference their request later.
        return new SubmitWebFormResponse(ticket.Id, ticket.Id.ToString("N"));
    }
}
