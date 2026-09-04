using CustomerSupportCRM.Application.Features.RolePermissions;
using CustomerSupportCRM.Application.Features.RolePermissions.Dtos;
using CustomerSupportCRM.Domain.Constants;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CustomerSupportCRM.Infrastructure.Identity.Services;

/// <summary>
/// Admin read/write access to the permission catalog and role-permission
/// assignments (security-admin/manage-role-permissions). Uses the generic
/// repository/unit-of-work pattern for <see cref="RolePermission"/> writes, and
/// <see cref="RoleManager{TRole}"/> only to resolve role id/name. Evicts the
/// affected role's <see cref="PermissionService"/> cache entry synchronously
/// after every write so authorization checks see the change immediately.
/// </summary>
public sealed class RolePermissionAdminService : IRolePermissionAdminService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _cache;

    public RolePermissionAdminService(RoleManager<ApplicationRole> roleManager, IUnitOfWork unitOfWork, IMemoryCache cache)
    {
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public IReadOnlyCollection<string> GetCatalog() => Permissions.All;

    public async Task<RolePermissionsDto> GetRolePermissionsAsync(Guid roleId, CancellationToken ct)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null)
        {
            throw new NotFoundException(nameof(ApplicationRole), roleId);
        }

        var repository = _unitOfWork.Repository<RolePermission>();
        var permissions = (await repository.FindAsync(rp => rp.RoleId == roleId, ct))
            .Select(rp => rp.PermissionName)
            .Distinct()
            .OrderBy(p => p, StringComparer.Ordinal)
            .ToList();

        return new RolePermissionsDto(roleId, role.Name!, permissions);
    }

    public async Task AssignPermissionAsync(Guid roleId, string permissionName, CancellationToken ct)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null)
        {
            throw new NotFoundException(nameof(ApplicationRole), roleId);
        }

        var repository = _unitOfWork.Repository<RolePermission>();
        var exists = await repository.ExistsAsync(
            rp => rp.RoleId == roleId && rp.PermissionName == permissionName, ct);

        if (exists)
        {
            throw new ConflictException($"Role already has permission '{permissionName}'.");
        }

        try
        {
            await repository.AddAsync(new RolePermission { RoleId = roleId, PermissionName = permissionName }, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            // Unique-index race: another request assigned the same pair concurrently.
            throw new ConflictException($"Role already has permission '{permissionName}'.");
        }

        _cache.Remove(PermissionCacheKeys.ForRoleName(role.Name!));
    }

    public async Task RemovePermissionAsync(Guid roleId, string permissionName, CancellationToken ct)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null)
        {
            throw new NotFoundException(nameof(ApplicationRole), roleId);
        }

        // The Admin role must always keep every permission - PermissionSeeder
        // re-grants them on next startup anyway, but block the API call
        // explicitly to avoid a self-lockout window before that happens.
        if (string.Equals(role.Name, Roles.Admin, StringComparison.Ordinal))
        {
            throw new ConflictException("Permissions cannot be removed from the Admin role.");
        }

        var repository = _unitOfWork.Repository<RolePermission>();
        var existing = (await repository.FindAsync(
            rp => rp.RoleId == roleId && rp.PermissionName == permissionName, ct)).FirstOrDefault();

        if (existing == null)
        {
            return; // idempotent: already absent
        }

        repository.Delete(existing);
        await _unitOfWork.SaveChangesAsync(ct);

        _cache.Remove(PermissionCacheKeys.ForRoleName(role.Name!));
    }
}
