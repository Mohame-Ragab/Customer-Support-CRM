using FluentValidation;

namespace CustomerSupportCRM.Application.Features.Chat.Commands.PostChatMessage;

public sealed class PostChatMessageCommandValidator : AbstractValidator<PostChatMessageCommand>
{
    public PostChatMessageCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
        RuleFor(x => x.Body).NotEmpty().MaximumLength(4000);
    }
}
