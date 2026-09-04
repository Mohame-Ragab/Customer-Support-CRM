namespace CustomerSupportCRM.Application.Features.Branding.Dtos;

public sealed record UpdateBrandingRequest(
    string PrimaryColor,
    string SecondaryColor,
    string? LogoBase64,        // raw base64 payload (no data-url prefix)
    string? LogoContentType);  // required iff LogoBase64 provided
