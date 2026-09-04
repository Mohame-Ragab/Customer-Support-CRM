namespace CustomerSupportCRM.Application.Features.Departments.DTOs;

public sealed record UpdateDepartmentRequest(string Name, string? Code, string? Description, bool IsActive);
