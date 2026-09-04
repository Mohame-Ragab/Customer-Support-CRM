using CustomerSupportCRM.Application.Features.Departments.DTOs;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;

namespace CustomerSupportCRM.Application.Features.Departments;

/// <summary>
/// Admin CRUD for the Department aggregate (platform/multi-department-support).
/// Plain injected service (not MediatR), matching the pattern already used by
/// <c>ISystemConfigurationService</c> - reads are open to any authenticated
/// user, writes are Administrator-only (enforced by the controller's
/// [Authorize] attributes, not here).
/// Application cannot reference EF Core directly, so the last line of defense
/// against a race (two concurrent creates with the same name) is the DB's own
/// unique index - it fails with a generic 500 in that narrow window rather
/// than a 409, which is an accepted trade-off given this is a low-frequency
/// admin action (see also BrandingService's concurrency note for the same
/// pattern elsewhere in F12).
/// </summary>
public sealed class DepartmentService : IDepartmentService
{
    private readonly IGenericRepository<Department> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DepartmentService(IGenericRepository<Department> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<DepartmentDto>> GetListAsync(bool includeInactive, CancellationToken ct)
    {
        var departments = includeInactive
            ? await _repository.GetAllAsync(ct)
            : await _repository.FindAsync(d => d.IsActive, ct);

        return departments.OrderBy(d => d.Name, StringComparer.OrdinalIgnoreCase).Select(ToDto).ToList();
    }

    public async Task<DepartmentDto> GetByIdAsync(Guid id, bool includeInactive, CancellationToken ct)
    {
        var department = await _repository.GetByIdAsync(id, ct);
        if (department == null || (!includeInactive && !department.IsActive))
        {
            throw new NotFoundException(nameof(Department), id);
        }

        return ToDto(department);
    }

    public async Task<DepartmentDto> CreateAsync(CreateDepartmentRequest request, CancellationToken ct)
    {
        var name = request.Name.Trim();
        var code = string.IsNullOrWhiteSpace(request.Code) ? null : request.Code.Trim();

        if (await NameExistsAsync(name, excludeId: null, ct))
        {
            throw new ConflictException($"A department named '{name}' already exists.");
        }

        if (code != null && await CodeExistsAsync(code, excludeId: null, ct))
        {
            throw new ConflictException($"A department with code '{code}' already exists.");
        }

        var department = new Department
        {
            Name = name,
            Code = code,
            Description = request.Description?.Trim(),
            IsActive = true,
        };

        await _repository.AddAsync(department, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return ToDto(department);
    }

    public async Task<DepartmentDto> UpdateAsync(Guid id, UpdateDepartmentRequest request, CancellationToken ct)
    {
        var department = await _repository.GetByIdAsync(id, ct);
        if (department == null)
        {
            throw new NotFoundException(nameof(Department), id);
        }

        var name = request.Name.Trim();
        var code = string.IsNullOrWhiteSpace(request.Code) ? null : request.Code.Trim();

        if (await NameExistsAsync(name, excludeId: id, ct))
        {
            throw new ConflictException($"A department named '{name}' already exists.");
        }

        if (code != null && await CodeExistsAsync(code, excludeId: id, ct))
        {
            throw new ConflictException($"A department with code '{code}' already exists.");
        }

        // Reactivation is allowed: an Administrator updating a soft-deleted
        // department with IsActive = true brings it back.
        department.Name = name;
        department.Code = code;
        department.Description = request.Description?.Trim();
        department.IsActive = request.IsActive;

        _repository.Update(department);
        await _unitOfWork.SaveChangesAsync(ct);

        return ToDto(department);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var department = await _repository.GetByIdAsync(id, ct);
        if (department == null)
        {
            throw new NotFoundException(nameof(Department), id);
        }

        if (!department.IsActive)
        {
            return; // idempotent: already soft-deleted
        }

        department.IsActive = false;
        _repository.Update(department);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    private async Task<bool> NameExistsAsync(string name, Guid? excludeId, CancellationToken ct)
    {
        var matches = await _repository.FindAsync(
            d => d.IsActive && d.Name.ToLower() == name.ToLower(), ct);
        return matches.Any(d => d.Id != excludeId);
    }

    private async Task<bool> CodeExistsAsync(string code, Guid? excludeId, CancellationToken ct)
    {
        var matches = await _repository.FindAsync(
            d => d.IsActive && d.Code != null && d.Code.ToLower() == code.ToLower(), ct);
        return matches.Any(d => d.Id != excludeId);
    }

    private static DepartmentDto ToDto(Department d) => new(
        d.Id, d.Name, d.Code, d.Description, d.IsActive, d.CreatedAt, d.UpdatedAt);
}
