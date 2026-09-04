using AutoMapper;
using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Features.Chat.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Chat.Queries;

public sealed class GetChatMessagesQueryHandler
    : IRequestHandler<GetChatMessagesQuery, IReadOnlyList<ChatMessageDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetChatMessagesQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ChatMessageDto>> Handle(
        GetChatMessagesQuery request, CancellationToken cancellationToken)
    {
        var session = await _unitOfWork.Repository<ChatSession>().GetByIdAsync(request.SessionId, cancellationToken);
        if (session is null)
        {
            throw new NotFoundException(nameof(ChatSession), request.SessionId);
        }

        var userId = _currentUserService.UserId;
        var isStaff = _currentUserService.Roles.Any(r =>
            r is "Agent" or "Supervisor" or "Manager" or "Admin");
        var isSessionCustomer = userId.HasValue && session.CustomerUserId == userId;
        var isAssignedAgent = userId.HasValue && session.AssignedAgentUserId == userId;
        var isUnassignedAndStaff = isStaff && session.AssignedAgentUserId is null;

        if (!isSessionCustomer && !isAssignedAgent && !isUnassignedAndStaff)
        {
            throw new ForbiddenAccessException("You are not a participant in this chat session.");
        }

        var query = _unitOfWork.Repository<ChatMessage>().Query()
            .Where(m => m.ChatSessionId == request.SessionId);

        if (request.BeforeUtc.HasValue)
        {
            query = query.Where(m => m.SentAtUtc < request.BeforeUtc.Value);
        }

        var take = request.Take is > 0 and <= 200 ? request.Take : 50;

        var messages = query
            .OrderByDescending(m => m.SentAtUtc)
            .Take(take)
            .ToList()
            .OrderBy(m => m.SentAtUtc)
            .ToList();

        return _mapper.Map<List<ChatMessageDto>>(messages);
    }
}
