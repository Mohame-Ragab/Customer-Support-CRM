using AutoMapper;
using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Features.Chat.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Enums;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Chat.Commands.PostChatMessage;

public sealed class PostChatMessageCommandHandler : IRequestHandler<PostChatMessageCommand, ChatMessageDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IChatNotifier _chatNotifier;
    private readonly IMapper _mapper;

    public PostChatMessageCommandHandler(
        IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IChatNotifier chatNotifier, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _chatNotifier = chatNotifier;
        _mapper = mapper;
    }

    public async Task<ChatMessageDto> Handle(PostChatMessageCommand request, CancellationToken cancellationToken)
    {
        var session = await _unitOfWork.Repository<ChatSession>().GetByIdAsync(request.SessionId, cancellationToken);
        if (session is null)
        {
            throw new NotFoundException(nameof(ChatSession), request.SessionId);
        }

        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedException("Chat requires an authenticated user.");

        var isAgent = _currentUserService.Roles.Any(r =>
            r is "Agent" or "Supervisor" or "Manager" or "Admin");
        var isSessionCustomer = session.CustomerUserId == userId;

        if (!isAgent && !isSessionCustomer)
        {
            throw new ForbiddenAccessException("You are not a participant in this chat session.");
        }

        var kind = isSessionCustomer ? ChatMessageKind.Customer : ChatMessageKind.Agent;

        var message = new ChatMessage
        {
            ChatSessionId = session.Id,
            TicketId = session.TicketId,
            SenderUserId = userId,
            Kind = kind,
            Body = request.Body,
            SentAtUtc = DateTime.UtcNow,
        };

        await _unitOfWork.Repository<ChatMessage>().AddAsync(message, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<ChatMessageDto>(message);
        await _chatNotifier.NotifyMessagePostedAsync(session.Id, dto, cancellationToken);

        return dto;
    }
}
