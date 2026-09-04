using CustomerSupportCRM.Application.Features.Chat.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Chat.Commands.AssignChatSession;

/// <summary>The caller (an authenticated agent) assigns themselves to an unassigned session.</summary>
public sealed record AssignChatSessionCommand(Guid SessionId) : IRequest<ChatSessionDto>;
