using AutoMapper;
using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Features.Chat.Dtos;
using CustomerSupportCRM.Application.Features.Tickets.Commands.CreateTicket;
using CustomerSupportCRM.Application.Features.Tickets.CustomerResolution;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Enums;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Chat.Commands.StartChatSession;

public sealed class StartChatSessionCommandHandler : IRequestHandler<StartChatSessionCommand, ChatSessionDto>
{
    private const string NoAgentAvailableMessage =
        "No agent is available right now. We have opened a support ticket and an agent will reply shortly.";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly ICustomerResolver _customerResolver;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAgentPresenceTracker _presenceTracker;
    private readonly IChatNotifier _chatNotifier;
    private readonly IMapper _mapper;

    public StartChatSessionCommandHandler(
        IUnitOfWork unitOfWork,
        IMediator mediator,
        ICustomerResolver customerResolver,
        ICurrentUserService currentUserService,
        IAgentPresenceTracker presenceTracker,
        IChatNotifier chatNotifier,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _customerResolver = customerResolver;
        _currentUserService = currentUserService;
        _presenceTracker = presenceTracker;
        _chatNotifier = chatNotifier;
        _mapper = mapper;
    }

    public async Task<ChatSessionDto> Handle(StartChatSessionCommand request, CancellationToken cancellationToken)
    {
        var customerUserId = _currentUserService.UserId
            ?? throw new Domain.Exceptions.UnauthorizedException("Chat requires an authenticated customer.");

        // A customer opening a second session while one is already open gets
        // the existing session back rather than a duplicate.
        var existing = await _unitOfWork.Repository<ChatSession>().FindAsync(
            s => s.CustomerUserId == customerUserId && s.EndedAtUtc == null, cancellationToken);
        var openSession = existing.FirstOrDefault();
        if (openSession is not null)
        {
            return _mapper.Map<ChatSessionDto>(openSession);
        }

        var email = _currentUserService.Email ?? $"{customerUserId}@unknown.local";
        var customerId = await _customerResolver.ResolveForApplicationUserAsync(
            customerUserId, email, _currentUserService.UserName, cancellationToken);

        var ticket = await _mediator.Send(
            new CreateTicketCommand("Live chat session", null, customerId, TicketChannel.LiveChat),
            cancellationToken);

        var session = new ChatSession
        {
            TicketId = ticket.Id,
            CustomerUserId = customerUserId,
            StartedAtUtc = DateTime.UtcNow,
        };

        await _unitOfWork.Repository<ChatSession>().AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (!_presenceTracker.AnyAgentsOnline())
        {
            var systemMessage = new ChatMessage
            {
                ChatSessionId = session.Id,
                TicketId = ticket.Id,
                SenderUserId = null,
                Kind = ChatMessageKind.System,
                Body = NoAgentAvailableMessage,
                SentAtUtc = DateTime.UtcNow,
            };

            await _unitOfWork.Repository<ChatMessage>().AddAsync(systemMessage, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var dto = _mapper.Map<ChatSessionDto>(session);
        await _chatNotifier.NotifySessionOpenedAsync(dto, cancellationToken);

        return dto;
    }
}
