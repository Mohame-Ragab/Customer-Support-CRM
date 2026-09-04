namespace CustomerSupportCRM.Application.Features.SystemConfiguration.Dtos;

public sealed class SystemSettingDto
{
    public Guid Id { get; init; }
    public string Key { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string ValueType { get; init; } = "String";
    public string? Description { get; init; }
    public bool IsRequired { get; init; }
}
