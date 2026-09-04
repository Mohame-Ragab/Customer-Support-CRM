using CustomerSupportCRM.API.Authorization;
using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Authorization;

namespace CustomerSupportCRM.API.Extensions;

/// <summary>
/// Registers the permission-based authorization pipeline (security-admin/manage-role-permissions):
/// the dynamic policy provider, its requirement handler, the permission
/// resolver, and the memory cache it uses. Call right after
/// <c>AddJwtAuthentication</c> in Program.cs.
/// </summary>
public static class PermissionAuthorizationExtensions
{
    public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        return services;
    }
}
