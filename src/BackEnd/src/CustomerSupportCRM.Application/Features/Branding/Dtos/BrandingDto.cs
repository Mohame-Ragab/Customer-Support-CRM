namespace CustomerSupportCRM.Application.Features.Branding.Dtos;

public sealed record BrandingDto(
    string PrimaryColor,
    string SecondaryColor,
    string? LogoDataUrl); // "data:{contentType};base64,{...}" or null
