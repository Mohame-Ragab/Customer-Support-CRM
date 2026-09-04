using CustomerSupportCRM.Application.Features.Branding.Dtos;
using FluentValidation;

namespace CustomerSupportCRM.Application.Features.Branding.Validators;

public sealed class UpdateBrandingRequestValidator : AbstractValidator<UpdateBrandingRequest>
{
    private const int MaxLogoBytes = 512 * 1024; // 512 KB decoded

    private static readonly string[] AllowedContentTypes =
    {
        "image/png", "image/jpeg", "image/svg+xml", "image/webp",
    };

    public UpdateBrandingRequestValidator()
    {
        RuleFor(x => x.PrimaryColor)
            .NotEmpty()
            .Matches(@"^#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6}|[0-9a-fA-F]{8})$");

        RuleFor(x => x.SecondaryColor)
            .NotEmpty()
            .Matches(@"^#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6}|[0-9a-fA-F]{8})$");

        RuleFor(x => x.LogoBase64)
            .Must(BeValidBase64WithinSizeLimit)
            .WithMessage($"Logo must be valid base64 and decode to at most {MaxLogoBytes / 1024} KB.")
            .When(x => !string.IsNullOrWhiteSpace(x.LogoBase64));

        RuleFor(x => x.LogoContentType)
            .NotEmpty()
            .Must(ct => ct != null && AllowedContentTypes.Contains(ct))
            .WithMessage("Logo content type must be one of: " + string.Join(", ", AllowedContentTypes))
            .When(x => !string.IsNullOrWhiteSpace(x.LogoBase64));
    }

    private static bool BeValidBase64WithinSizeLimit(string? base64)
    {
        if (string.IsNullOrWhiteSpace(base64))
        {
            return true;
        }

        try
        {
            var bytes = Convert.FromBase64String(base64);
            return bytes.Length <= MaxLogoBytes;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
