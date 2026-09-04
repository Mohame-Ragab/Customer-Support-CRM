using MediatR;

namespace CustomerSupportCRM.Application.Features.QuickReplies;

public sealed record CreateQuickReplyTemplateCommand(string Name, string Body) : IRequest<QuickReplyTemplateDto>;
