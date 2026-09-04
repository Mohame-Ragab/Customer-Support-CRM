using CustomerSupportCRM.Application.Features.Chat.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Chat.Queries;

public sealed record GetChatMessagesQuery(Guid SessionId, int Take = 50, DateTime? BeforeUtc = null)
    : IRequest<IReadOnlyList<ChatMessageDto>>;
