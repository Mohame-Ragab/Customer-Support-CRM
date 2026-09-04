using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Features.Chat.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace CustomerSupportCRM.API.Hubs;

/// <summary>
/// Pushes chat events to connected clients via the SignalR hub. See
/// <see cref="IChatNotifier"/> for why this lives in API instead of
/// Infrastructure.
/// </summary>
public sealed class ChatNotifier : IChatNotifier
{
    private readonly IHubContext<ChatHub, IChatClient> _hubContext;

    public ChatNotifier(IHubContext<ChatHub, IChatClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyMessagePostedAsync(Guid sessionId, ChatMessageDto message, CancellationToken cancellationToken)
        => _hubContext.Clients.Group(ChatHub.SessionGroupName(sessionId)).MessagePosted(message);

    public Task NotifySessionAssignedAsync(Guid sessionId, Guid agentUserId, CancellationToken cancellationToken)
        => _hubContext.Clients.Group(ChatHub.SessionGroupName(sessionId)).SessionAssigned(sessionId, agentUserId);

    public Task NotifySessionOpenedAsync(ChatSessionDto session, CancellationToken cancellationToken)
        => _hubContext.Clients.Group(ChatHub.AgentsLobbyGroup).SessionOpened(session);
}
