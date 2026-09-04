using MediatR;

namespace CustomerSupportCRM.Application.Features.QuickReplies;

public sealed record UpdateQuickReplyTemplateCommand(Guid Id, string Name, string Body) : IRequest<QuickReplyTemplateDto>;
