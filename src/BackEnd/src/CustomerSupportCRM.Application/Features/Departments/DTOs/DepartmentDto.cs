namespace CustomerSupportCRM.Application.Features.Departments.DTOs;

public sealed record DepartmentDto(
    Guid Id,
    string Name,
    string? Code,
    string? Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
