namespace CustomerSupportCRM.Application.Common.Models;

/// <summary>Strongly typed binding of the "WebForms" configuration section (F03 web-forms-channel).</summary>
public sealed class WebFormsSettings
{
    public const string SectionName = "WebForms";

    public int RateLimitPermitPerMinute { get; init; } = 5;
}
