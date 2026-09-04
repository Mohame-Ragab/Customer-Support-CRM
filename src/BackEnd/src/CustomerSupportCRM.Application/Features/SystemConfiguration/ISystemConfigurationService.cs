using CustomerSupportCRM.Application.Features.SystemConfiguration.Dtos;

namespace CustomerSupportCRM.Application.Features.SystemConfiguration;

public interface ISystemConfigurationService
{
    Task<IReadOnlyList<SystemSettingDto>> GetAllAsync(CancellationToken ct = default);

    Task<SystemSettingDto> GetByKeyAsync(string key, CancellationToken ct = default);

    Task<SystemSettingDto> UpdateAsync(string key, UpdateSystemSettingDto dto, CancellationToken ct = default);
}
