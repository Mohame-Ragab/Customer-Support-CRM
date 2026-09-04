using MediatR;

namespace CustomerSupportCRM.Application.Features.QuickReplies;

public sealed record GetMyQuickReplyTemplatesQuery(string? Search = null) : IRequest<IReadOnlyList<QuickReplyTemplateDto>>;
