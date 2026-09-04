using CustomerSupportCRM.Application.Features.Chat.Dtos;

namespace CustomerSupportCRM.Application.Common.Interfaces;

/// <summary>
/// Abstraction command handlers use to push real-time chat events. Implemented
/// in the API project (not Infrastructure, unlike most ports) because the
/// concrete implementation needs <c>IHubContext&lt;ChatHub, IChatClient&gt;</c>,
/// and both the hub and its strongly-typed client interface are ASP.NET Core
/// SignalR hosting types that only make sense in the Web (API) project -
/// Infrastructure must not reference API (wrong Onion direction). This is a
/// deliberate placement deviation from the story's original suggestion.
/// </summary>
public interface IChatNotifier
{
    Task NotifyMessagePostedAsync(Guid sessionId, ChatMessageDto message, CancellationToken cancellationToken);

    Task NotifySessionAssignedAsync(Guid sessionId, Guid agentUserId, CancellationToken cancellationToken);

    Task NotifySessionOpenedAsync(ChatSessionDto session, CancellationToken cancellationToken);
}
