using CustomerSupportCRM.Application.Features.KnowledgeBase.Dtos;
using CustomerSupportCRM.Domain.Enums;
using MediatR;

namespace CustomerSupportCRM.Application.Features.KnowledgeBase.Commands;

public sealed record UpdateKnowledgeBaseContentCommand(
    Guid Id, string Title, string Body, string? Summary, ContentType Type, string? Language, bool? IsPublished)
    : IRequest<KnowledgeBaseContentDto>;
