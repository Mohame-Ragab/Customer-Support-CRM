namespace CustomerSupportCRM.Domain.Constants;

/// <summary>
/// Strongly-typed audit action names (security-admin/view-audit-logs). Every
/// F10 command handler that mutates state calls
/// <c>IAuditLogService.WriteAsync</c> with one of these - no magic strings.
/// </summary>
public static class AuditActions
{
    public const string UserCreated = "User.Created";
    public const string UserUpdated = "User.Updated";
    public const string UserDeleted = "User.Deleted";
    public const string RoleCreated = "Role.Created";
    public const string RoleUpdated = "Role.Updated";
    public const string RoleDeleted = "Role.Deleted";
    public const string RoleAssigned = "Role.Assigned";
    public const string RolePermissionsChanged = "RolePermissions.Changed";
    public const string SystemConfigurationChanged = "SystemConfiguration.Changed";
}
