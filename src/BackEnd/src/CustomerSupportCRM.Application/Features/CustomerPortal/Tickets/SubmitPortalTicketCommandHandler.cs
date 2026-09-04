using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Features.Tickets.Commands.CreateTicket;
using CustomerSupportCRM.Application.Features.Tickets.CustomerResolution;
using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using CustomerSupportCRM.Domain.Enums;
using CustomerSupportCRM.Domain.Exceptions;
using MediatR;

namespace CustomerSupportCRM.Application.Features.CustomerPortal.Tickets;

public sealed class SubmitPortalTicketCommandHandler : IRequestHandler<SubmitPortalTicketCommand, TicketDto>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ICustomerResolver _customerResolver;
    private readonly IMediator _mediator;

    public SubmitPortalTicketCommandHandler(
        ICurrentUserService currentUserService, ICustomerResolver customerResolver, IMediator mediator)
    {
        _currentUserService = currentUserService;
        _customerResolver = customerResolver;
        _mediator = mediator;
    }

    public async Task<TicketDto> Handle(SubmitPortalTicketCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedException("Submitting a ticket requires an authenticated customer.");

        var email = _currentUserService.Email
            ?? throw new UnauthorizedException("Submitting a ticket requires a verified email address.");

        var customerId = await _customerResolver.ResolveForApplicationUserAsync(
            userId, email, _currentUserService.UserName, cancellationToken);

        return await _mediator.Send(
            new CreateTicketCommand(request.Subject, request.Description, customerId, TicketChannel.CustomerPortal),
            cancellationToken);
    }
}
