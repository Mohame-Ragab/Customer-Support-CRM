using CustomerSupportCRM.Application.Common.Interfaces;

namespace CustomerSupportCRM.API.Hubs;

/// <summary>
/// In-memory count of currently-connected staff SignalR connections (the
/// "agents-lobby" group). Singleton, updated by <see cref="ChatHub"/>'s
/// connect/disconnect lifecycle. A single-process counter is sufficient for
/// this project (no multi-instance/backplane deployment yet).
/// </summary>
public sealed class AgentPresenceTracker : IAgentPresenceTracker
{
    private int _connectedAgentCount;

    public bool AnyAgentsOnline() => Volatile.Read(ref _connectedAgentCount) > 0;

    public void AgentConnected() => Interlocked.Increment(ref _connectedAgentCount);

    public void AgentDisconnected() => Interlocked.Decrement(ref _connectedAgentCount);
}
