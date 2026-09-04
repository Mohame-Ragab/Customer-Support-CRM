namespace CustomerSupportCRM.Application.Common.Interfaces;

/// <summary>
/// Whether any staff (Agent/Supervisor/Manager/Admin) SignalR connection is
/// currently in the chat hub's "agents-lobby" group. Implemented in the API
/// project alongside <c>ChatHub</c> (same reasoning as <see cref="IChatNotifier"/>
/// - connection/group membership is an ASP.NET Core SignalR hosting concern).
/// Used by StartChatSessionCommandHandler to decide whether to post the
/// "no agent available" system message.
/// </summary>
public interface IAgentPresenceTracker
{
    bool AnyAgentsOnline();
}
