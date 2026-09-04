using CustomerSupportCRM.Application.Features.Departments.DTOs;

namespace CustomerSupportCRM.Application.Features.Departments;

public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentDto>> GetListAsync(bool includeInactive, CancellationToken ct);

    Task<DepartmentDto> GetByIdAsync(Guid id, bool includeInactive, CancellationToken ct);

    Task<DepartmentDto> CreateAsync(CreateDepartmentRequest request, CancellationToken ct);

    Task<DepartmentDto> UpdateAsync(Guid id, UpdateDepartmentRequest request, CancellationToken ct);

    Task DeleteAsync(Guid id, CancellationToken ct);
}
