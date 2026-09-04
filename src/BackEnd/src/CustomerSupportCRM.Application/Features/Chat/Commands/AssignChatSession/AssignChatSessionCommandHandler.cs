using AutoMapper;
using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Features.Chat.Dtos;
using CustomerSupportCRM.Application.Features.Tickets.History;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Enums;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Chat.Commands.AssignChatSession;

public sealed class AssignChatSessionCommandHandler : IRequestHandler<AssignChatSessionCommand, ChatSessionDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITicketHistoryWriter _historyWriter;
    private readonly IChatNotifier _chatNotifier;
    private readonly IMapper _mapper;

    public AssignChatSessionCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ITicketHistoryWriter historyWriter,
        IChatNotifier chatNotifier,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _historyWriter = historyWriter;
        _chatNotifier = chatNotifier;
        _mapper = mapper;
    }

    public async Task<ChatSessionDto> Handle(AssignChatSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _unitOfWork.Repository<ChatSession>().GetByIdAsync(request.SessionId, cancellationToken);
        if (session is null)
        {
            throw new NotFoundException(nameof(ChatSession), request.SessionId);
        }

        var agentUserId = _currentUserService.UserId
            ?? throw new UnauthorizedException("Chat assignment requires an authenticated agent.");

        if (session.AssignedAgentUserId == agentUserId)
        {
            return _mapper.Map<ChatSessionDto>(session);
        }

        if (session.AssignedAgentUserId is not null)
        {
            throw new ConflictException("This chat session is already assigned to another agent.");
        }

        session.AssignedAgentUserId = agentUserId;
        _unitOfWork.Repository<ChatSession>().Update(session);

        var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(session.TicketId, cancellationToken);
        if (ticket is not null && ticket.AssignedAgentId != agentUserId)
        {
            var oldAgent = ticket.AssignedAgentId;
            ticket.AssignedAgentId = agentUserId;
            ticket.AssignedAt = DateTime.UtcNow;
            _unitOfWork.Repository<Ticket>().Update(ticket);

            await _historyWriter.AppendAsync(
                ticket.Id,
                TicketHistoryEventType.AssignmentChanged,
                oldValue: oldAgent?.ToString(),
                newValue: agentUserId.ToString(),
                note: "Assigned via live chat",
                cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<ChatSessionDto>(session);
        await _chatNotifier.NotifySessionAssignedAsync(session.Id, agentUserId, cancellationToken);

        return dto;
    }
}
