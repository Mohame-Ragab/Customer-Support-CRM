using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Infrastructure.Identity.Services;
using CustomerSupportCRM.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerSupportCRM.Infrastructure.Identity;

/// <summary>
/// Wires ASP.NET Core Identity (<see cref="ApplicationUser"/>/<see cref="ApplicationRole"/>)
/// to <see cref="ApplicationDbContext"/>. Isolated from
/// <see cref="DependencyInjection.AddInfrastructure"/> so Identity's registration
/// concerns (password/lockout policy, token providers, etc.) stay in one place.
/// </summary>
public static class IdentityExtensions
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services)
    {
        services
            .AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Baseline password policy; revisit once real user-management
                // features (registration, admin reset, etc.) are implemented.
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<DataProtectionTokenProviderOptions>(o =>
            o.TokenLifespan = TimeSpan.FromHours(24));

        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IRegistrationService, RegistrationService>();
        services.AddScoped<IEmailVerificationService, EmailVerificationService>();

        return services;
    }
}
