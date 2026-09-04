using MediatR;

namespace CustomerSupportCRM.Application.Features.QuickReplies;

public sealed record DeleteQuickReplyTemplateCommand(Guid Id) : IRequest;
