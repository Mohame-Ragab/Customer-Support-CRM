using MediatR;

namespace CustomerSupportCRM.Application.Features.KnowledgeBase.Commands;

public sealed record DeleteKnowledgeBaseContentCommand(Guid Id) : IRequest;
