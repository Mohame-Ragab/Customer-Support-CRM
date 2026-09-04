using CustomerSupportCRM.Application.Features.Branding.Dtos;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Interfaces;

namespace CustomerSupportCRM.Application.Features.Branding;

/// <summary>
/// Singleton branding row read/write (platform/custom-branding). Looked up as
/// "the only row" rather than by a specific id, so Application does not need
/// to know Infrastructure's seeded row id - see
/// BrandingSettingsConfiguration.SingletonId for where that id lives.
/// Tolerates a missing row (falls back to <see cref="BrandingSettings"/>'s
/// own defaults) so a manually deleted seed row, or a rollback that dropped
/// the table before the API redeployed, never breaks the public endpoint.
/// Concurrent updates are last-write-wins by design - branding is a
/// low-frequency admin action; no optimistic-concurrency token is used.
/// </summary>
public sealed class BrandingService : IBrandingService
{
    private readonly IGenericRepository<BrandingSettings> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public BrandingService(IGenericRepository<BrandingSettings> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BrandingDto> GetAsync(CancellationToken ct)
    {
        var settings = (await _repository.GetAllAsync(ct)).FirstOrDefault() ?? new BrandingSettings();
        return ToDto(settings);
    }

    public async Task<BrandingDto> UpdateAsync(UpdateBrandingRequest request, CancellationToken ct)
    {
        var settings = (await _repository.GetAllAsync(ct)).FirstOrDefault();

        if (settings == null)
        {
            settings = new BrandingSettings();
            await _repository.AddAsync(settings, ct);
        }

        settings.PrimaryColor = request.PrimaryColor;
        settings.SecondaryColor = request.SecondaryColor;

        if (!string.IsNullOrWhiteSpace(request.LogoBase64))
        {
            settings.LogoBytes = Convert.FromBase64String(request.LogoBase64);
            settings.LogoContentType = request.LogoContentType;
        }

        _repository.Update(settings);
        await _unitOfWork.SaveChangesAsync(ct);

        return ToDto(settings);
    }

    private static BrandingDto ToDto(BrandingSettings settings)
    {
        string? logoDataUrl = null;
        if (settings.LogoBytes is { Length: > 0 } && !string.IsNullOrWhiteSpace(settings.LogoContentType))
        {
            logoDataUrl = $"data:{settings.LogoContentType};base64,{Convert.ToBase64String(settings.LogoBytes)}";
        }

        return new BrandingDto(settings.PrimaryColor, settings.SecondaryColor, logoDataUrl);
    }
}
