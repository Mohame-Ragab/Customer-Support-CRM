using CustomerSupportCRM.Application.Features.Chat.Commands.AssignChatSession;
using CustomerSupportCRM.Application.Features.Chat.Commands.PostChatMessage;
using CustomerSupportCRM.Domain.Constants;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CustomerSupportCRM.API.Hubs;

/// <summary>
/// Real-time transport for live chat (F03 live-chat-communication-channel).
/// Persistence and broadcast are delegated to the Application layer
/// (IMediator + IChatNotifier, called from the command handlers) - this hub
/// only handles connection lifecycle/group membership and translates hub
/// method calls into MediatR commands.
/// </summary>
[Authorize]
public sealed class ChatHub : Hub<IChatClient>
{
    public const string AgentsLobbyGroup = "agents-lobby";

    public static string SessionGroupName(Guid sessionId) => $"chat-session-{sessionId:N}";

    private readonly IMediator _mediator;
    private readonly IValidator<PostChatMessageCommand> _postMessageValidator;
    private readonly AgentPresenceTracker _presenceTracker;

    public ChatHub(
        IMediator mediator,
        IValidator<PostChatMessageCommand> postMessageValidator,
        AgentPresenceTracker presenceTracker)
    {
        _mediator = mediator;
        _postMessageValidator = postMessageValidator;
        _presenceTracker = presenceTracker;
    }

    public override async Task OnConnectedAsync()
    {
        if (IsAgent())
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, AgentsLobbyGroup);
            _presenceTracker.AgentConnected();
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (IsAgent())
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, AgentsLobbyGroup);
            _presenceTracker.AgentDisconnected();
        }

        await base.OnDisconnectedAsync(exception);
    }

    public Task JoinSession(Guid sessionId)
        => Groups.AddToGroupAsync(Context.ConnectionId, SessionGroupName(sessionId));

    public Task LeaveSession(Guid sessionId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, SessionGroupName(sessionId));

    public async Task SendMessage(Guid sessionId, string body)
    {
        var command = new PostChatMessageCommand(sessionId, body);
        var validation = await _postMessageValidator.ValidateAsync(command, Context.ConnectionAborted);
        if (!validation.IsValid)
        {
            throw new HubException(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        try
        {
            await _mediator.Send(command, Context.ConnectionAborted);
        }
        catch (Exception ex) when (ex is Domain.Exceptions.DomainException)
        {
            // Surface a client-visible error instead of a raw 500-style disconnect;
            // HubException messages are the only exception detail SignalR sends to
            // the client in production (everything else is sanitized).
            throw new HubException(ex.Message);
        }
    }

    public async Task AssignToMe(Guid sessionId)
    {
        if (!IsAgent())
        {
            throw new HubException("Only staff can assign a chat session.");
        }

        try
        {
            await _mediator.Send(new AssignChatSessionCommand(sessionId), Context.ConnectionAborted);
        }
        catch (Exception ex) when (ex is Domain.Exceptions.DomainException)
        {
            throw new HubException(ex.Message);
        }
    }

    private bool IsAgent() =>
        Context.User?.IsInRole(Roles.Agent) == true ||
        Context.User?.IsInRole(Roles.Supervisor) == true ||
        Context.User?.IsInRole(Roles.Manager) == true ||
        Context.User?.IsInRole(Roles.Admin) == true;
}
