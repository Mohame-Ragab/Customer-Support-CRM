namespace CustomerSupportCRM.Application.Features.QuickReplies;

public sealed record QuickReplyTemplateDto(
    Guid Id, string Name, string Body, DateTime CreatedAt, DateTime? UpdatedAt);

public sealed record CreateQuickReplyTemplateRequest(string Name, string Body);

public sealed record UpdateQuickReplyTemplateRequest(string Name, string Body);
