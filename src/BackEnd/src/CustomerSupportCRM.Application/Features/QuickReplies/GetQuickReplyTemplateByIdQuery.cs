using MediatR;

namespace CustomerSupportCRM.Application.Features.QuickReplies;

public sealed record GetQuickReplyTemplateByIdQuery(Guid Id) : IRequest<QuickReplyTemplateDto>;
