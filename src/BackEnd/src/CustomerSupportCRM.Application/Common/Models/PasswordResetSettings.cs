namespace CustomerSupportCRM.Application.Common.Models;

public sealed class PasswordResetSettings
{
    public const string SectionName = "PasswordReset";

    /// <summary>Frontend base URL, e.g. https://app.example.com/reset-password</summary>
    public string ResetUrlBase { get; init; } = string.Empty;

    /// <summary>Optional; documented on the story as an open question. Default 60 minutes.</summary>
    public int TokenLifetimeMinutes { get; init; } = 60;
}
