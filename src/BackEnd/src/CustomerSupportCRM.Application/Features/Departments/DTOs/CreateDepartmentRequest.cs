namespace CustomerSupportCRM.Application.Features.Departments.DTOs;

public sealed record CreateDepartmentRequest(string Name, string? Code, string? Description);
