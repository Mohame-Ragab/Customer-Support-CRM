using CustomerSupportCRM.Application.Features.Chat.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Chat.Queries;

/// <summary>The agent lobby list: open (not ended), unassigned sessions.</summary>
public sealed record GetOpenChatSessionsQuery : IRequest<IReadOnlyList<ChatSessionDto>>;
