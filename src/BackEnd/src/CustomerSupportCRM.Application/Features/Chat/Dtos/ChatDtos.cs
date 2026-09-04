using CustomerSupportCRM.Domain.Enums;

namespace CustomerSupportCRM.Application.Features.Chat.Dtos;

public sealed record ChatSessionDto(
    Guid Id,
    Guid TicketId,
    Guid CustomerUserId,
    Guid? AssignedAgentUserId,
    DateTime StartedAtUtc,
    DateTime? EndedAtUtc);

public sealed record ChatMessageDto(
    Guid Id,
    Guid ChatSessionId,
    Guid TicketId,
    Guid? SenderUserId,
    ChatMessageKind Kind,
    string Body,
    DateTime SentAtUtc);
