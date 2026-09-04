namespace CustomerSupportCRM.Domain.Constants;

/// <summary>
/// Admin-configurable permission catalog (security-admin/manage-role-permissions).
/// Grouped in nested static classes per feature area, mirroring <see cref="Roles"/>.
/// The catalog is seeded via <c>PermissionSeeder</c> - no permission string is
/// hard-coded in feature code beyond this file.
/// </summary>
public static class Permissions
{
    public static class Tickets
    {
        public const string View = "tickets.view";
        public const string Create = "tickets.create";
        public const string Update = "tickets.update";
        public const string Assign = "tickets.assign";
        public const string Delete = "tickets.delete";
    }

    public static class Customers
    {
        public const string View = "customers.view";
        public const string Create = "customers.create";
        public const string Update = "customers.update";
    }

    public static class Roles
    {
        public const string Manage = "roles.manage";
    }

    public static class RolePermissions
    {
        public const string Manage = "rolepermissions.manage";
    }

    // Extend as the product-owned catalog grows.
    public static IReadOnlyCollection<string> All { get; } = new[]
    {
        Tickets.View, Tickets.Create, Tickets.Update, Tickets.Assign, Tickets.Delete,
        Customers.View, Customers.Create, Customers.Update,
        Roles.Manage,
        RolePermissions.Manage,
    };
}
