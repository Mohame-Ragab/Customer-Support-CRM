using CustomerSupportCRM.Application.Features.Chat.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Chat.Commands.PostChatMessage;

public sealed record PostChatMessageCommand(Guid SessionId, string Body) : IRequest<ChatMessageDto>;
