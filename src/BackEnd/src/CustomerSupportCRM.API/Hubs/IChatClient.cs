using CustomerSupportCRM.Application.Features.Chat.Dtos;

namespace CustomerSupportCRM.API.Hubs;

/// <summary>Strongly-typed SignalR client contract for <see cref="ChatHub"/>.</summary>
public interface IChatClient
{
    Task MessagePosted(ChatMessageDto message);

    Task SessionAssigned(Guid sessionId, Guid agentUserId);

    Task SessionOpened(ChatSessionDto session);
}
