using CustomerSupportCRM.Domain.Common;

namespace CustomerSupportCRM.Domain.Entities;

/// <summary>
/// Singleton branding row (Id == 1) - primary/secondary brand colors and an
/// optional logo image (platform/custom-branding). Kept as its own entity for
/// now; migrate into F10's generic key/value settings store
/// (security-admin/manage-system-configuration) when convenient - branding is
/// image-bearing (a byte[] logo), which doesn't fit that string-only schema
/// as-is, so this stays separate rather than forcing a fit.
/// </summary>
public class BrandingSettings : BaseEntity
{
    public string PrimaryColor { get; set; } = "#1565c0";   // defaults mirror styles/theme/palette.ts
    public string SecondaryColor { get; set; } = "#546e7a";
    public string? LogoContentType { get; set; }             // e.g. "image/png", "image/svg+xml"
    public byte[]? LogoBytes { get; set; }                   // null == no custom logo, use app name text
}
