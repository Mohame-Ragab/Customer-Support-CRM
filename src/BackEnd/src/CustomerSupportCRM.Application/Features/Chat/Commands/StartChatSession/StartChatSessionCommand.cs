using CustomerSupportCRM.Application.Features.Chat.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Chat.Commands.StartChatSession;

/// <summary>No body: the caller is always the authenticated Customer-role user starting their own session (see ICurrentUserService).</summary>
public sealed record StartChatSessionCommand : IRequest<ChatSessionDto>;
