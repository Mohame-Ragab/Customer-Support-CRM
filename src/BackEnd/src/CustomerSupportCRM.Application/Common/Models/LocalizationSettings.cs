namespace CustomerSupportCRM.Application.Common.Models;

/// <summary>
/// Strongly typed binding of the "Localization" configuration section, so the
/// supported/default cultures are configuration-driven rather than hardcoded in
/// <c>API/Localization/LocalizationExtensions</c>.
/// </summary>
public sealed class LocalizationSettings
{
    public const string SectionName = "Localization";

    public string DefaultCulture { get; init; } = "en";

    public string[] SupportedCultures { get; init; } = ["en", "ar"];
}
