using CustomerSupportCRM.Domain.Common;
using CustomerSupportCRM.Domain.Enums;

namespace CustomerSupportCRM.Domain.Entities;

/// <summary>
/// Admin-editable, database-backed key/value operational setting
/// (security-admin/manage-system-configuration). Distinct from the static
/// <c>ApplicationSettings</c> options binding: those values come from
/// appsettings.json at startup; these are persisted and mutable at runtime by
/// administrators. Extensible by design - the concrete list of settings is
/// intentionally open.
/// </summary>
public class SystemSetting : BaseEntity
{
    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public SystemSettingValueType ValueType { get; set; } = SystemSettingValueType.String;

    public string? Description { get; set; }

    public bool IsRequired { get; set; }
}
