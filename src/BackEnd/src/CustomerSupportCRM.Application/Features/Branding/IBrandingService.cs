using CustomerSupportCRM.Application.Features.Branding.Dtos;

namespace CustomerSupportCRM.Application.Features.Branding;

public interface IBrandingService
{
    Task<BrandingDto> GetAsync(CancellationToken ct);

    Task<BrandingDto> UpdateAsync(UpdateBrandingRequest request, CancellationToken ct);
}
