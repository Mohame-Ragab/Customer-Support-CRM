namespace CustomerSupportCRM.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<ChangePasswordOutcome> ChangePasswordAsync(
        string userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken);
}

public sealed record ChangePasswordOutcome(bool Succeeded, IReadOnlyList<string> Errors)
{
    public static ChangePasswordOutcome Success() => new(true, Array.Empty<string>());
    public static ChangePasswordOutcome Failure(IReadOnlyList<string> errors) => new(false, errors);
}
