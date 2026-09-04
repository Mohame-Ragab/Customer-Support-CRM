using CustomerSupportCRM.Domain.Common;

namespace CustomerSupportCRM.Domain.Entities;

/// <summary>
/// A department/team within the organization (platform/multi-department-support).
/// Foundational: other CRM entities (users, tickets, KB articles, reports) may
/// later reference a <c>DepartmentId</c> - wiring those references is out of
/// scope here. Deletion is soft (<see cref="IsActive"/>) so no downstream FK
/// ever cascade-fails.
/// </summary>
public class Department : BaseEntity
{
    public string Name { get; set; } = default!;

    public string? Code { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}
