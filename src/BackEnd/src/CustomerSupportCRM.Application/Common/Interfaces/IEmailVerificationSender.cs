namespace CustomerSupportCRM.Application.Common.Interfaces;

public interface IEmailVerificationSender
{
    Task QueueAsync(Guid userId, string email, CancellationToken cancellationToken);
}
