namespace CustomerSupportCRM.Application.Common.Models;

/// <summary>Strongly typed binding of the "ApplicationSettings" configuration section.</summary>
public sealed class ApplicationSettings
{
    public const string SectionName = "ApplicationSettings";

    public string ApplicationName { get; init; } = "Customer Support CRM";
}
