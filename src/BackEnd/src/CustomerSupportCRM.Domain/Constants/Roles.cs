namespace CustomerSupportCRM.Domain.Constants;

/// <summary>
/// Well-known role names used for role-based authorization across the system.
/// Defined once in Domain (plain strings, no Identity dependency) so Infrastructure
/// (role seeding) and API (authorization policies/attributes) share a single source
/// of truth instead of duplicating magic strings.
/// </summary>
public static class Roles
{
    public const string Admin = "Admin";
    public const string Supervisor = "Supervisor";
    public const string Manager = "Manager";
    public const string Agent = "Agent";
    public const string Customer = "Customer";
}

