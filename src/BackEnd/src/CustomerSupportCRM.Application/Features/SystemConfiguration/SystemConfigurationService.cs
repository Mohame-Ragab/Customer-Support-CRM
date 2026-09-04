using System.Globalization;
using System.Text.Json;
using CustomerSupportCRM.Application.Common.Exceptions;
using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Features.SystemConfiguration.Dtos;
using CustomerSupportCRM.Domain.Constants;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Enums;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using FluentValidation.Results;

namespace CustomerSupportCRM.Application.Features.SystemConfiguration;

/// <summary>
/// Admin read/write access to database-backed operational settings
/// (security-admin/manage-system-configuration). Lives in Application (unlike
/// the Identity-backed F10 services) because it depends only on the generic
/// repository/unit-of-work abstractions - no Infrastructure/Identity type is
/// needed.
/// </summary>
public sealed class SystemConfigurationService : ISystemConfigurationService
{
    private readonly IGenericRepository<SystemSetting> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogService _auditLogService;

    public SystemConfigurationService(
        IGenericRepository<SystemSetting> repository, IUnitOfWork unitOfWork, IAuditLogService auditLogService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _auditLogService = auditLogService;
    }

    public async Task<IReadOnlyList<SystemSettingDto>> GetAllAsync(CancellationToken ct = default)
    {
        var settings = await _repository.GetAllAsync(ct);
        return settings.OrderBy(s => s.Key, StringComparer.Ordinal).Select(ToDto).ToList();
    }

    public async Task<SystemSettingDto> GetByKeyAsync(string key, CancellationToken ct = default)
    {
        var setting = await FindByKeyAsync(key, ct);
        return ToDto(setting);
    }

    public async Task<SystemSettingDto> UpdateAsync(string key, UpdateSystemSettingDto dto, CancellationToken ct = default)
    {
        var setting = await FindByKeyAsync(key, ct);

        if (setting.IsRequired && string.IsNullOrWhiteSpace(dto.Value))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(dto.Value), $"Setting '{key}' is required and cannot be blank."),
            });
        }

        if (!string.IsNullOrWhiteSpace(dto.Value) && !IsValidForType(dto.Value, setting.ValueType))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(dto.Value), $"Value is not a valid {setting.ValueType} for setting '{key}'."),
            });
        }

        var previousValue = setting.Value;
        setting.Value = dto.Value;
        _repository.Update(setting);
        await _unitOfWork.SaveChangesAsync(ct);

        await _auditLogService.WriteAsync(
            AuditActions.SystemConfigurationChanged,
            entityType: "SystemSetting",
            entityId: setting.Id.ToString(),
            summary: $"Changed setting '{key}'.",
            metadata: new { Key = key, PreviousValue = previousValue, NewValue = dto.Value },
            cancellationToken: ct);

        return ToDto(setting);
    }

    private async Task<SystemSetting> FindByKeyAsync(string key, CancellationToken ct)
    {
        var matches = await _repository.FindAsync(s => s.Key == key, ct);
        var setting = matches.FirstOrDefault();
        if (setting == null)
        {
            throw new NotFoundException(nameof(SystemSetting), key);
        }

        return setting;
    }

    private static bool IsValidForType(string value, SystemSettingValueType type) => type switch
    {
        SystemSettingValueType.Integer => int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _),
        SystemSettingValueType.Boolean => bool.TryParse(value, out _),
        SystemSettingValueType.Decimal => decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out _),
        SystemSettingValueType.Json => IsValidJson(value),
        SystemSettingValueType.String => true,
        _ => true,
    };

    private static bool IsValidJson(string value)
    {
        try
        {
            using var _ = JsonDocument.Parse(value);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static SystemSettingDto ToDto(SystemSetting setting) => new()
    {
        Id = setting.Id,
        Key = setting.Key,
        Value = setting.Value,
        ValueType = setting.ValueType.ToString(),
        Description = setting.Description,
        IsRequired = setting.IsRequired,
    };
}
