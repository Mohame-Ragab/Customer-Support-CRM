using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.Users.Services;
using CustomerSupportCRM.Domain.Interfaces;
using CustomerSupportCRM.Infrastructure.Identity;
using CustomerSupportCRM.Infrastructure.Identity.Services;
using CustomerSupportCRM.Infrastructure.Persistence.Context;
using CustomerSupportCRM.Infrastructure.Persistence.Interceptors;
using CustomerSupportCRM.Infrastructure.Persistence.Repositories;
using CustomerSupportCRM.Infrastructure.Services;
using CustomerSupportCRM.Infrastructure.Services.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UnitOfWork = CustomerSupportCRM.Infrastructure.Persistence.UnitOfWork.UnitOfWork;

namespace CustomerSupportCRM.Infrastructure;

/// <summary>
/// Composition root entry point for the Infrastructure layer: EF Core + SQL
/// Server, ASP.NET Core Identity, the generic repository/unit of work, and the
/// audit interceptor. <see cref="ICurrentUserService"/> is consumed here but
/// implemented and registered by the API composition root - see
/// docs/architecture.md, "Dependency Injection".
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        // Registered here (not in Application.DependencyInjection) because AddAutoMapper
        // only honors the assemblies passed on its first call, and profiles such as
        // UserProfileMappingProfile map from Infrastructure/Identity types (e.g.
        // ApplicationUser) that the Application layer must not reference. Scanning
        // both assemblies in one call here - which Infrastructure can safely see -
        // registers every profile from both layers correctly.
        services.AddAutoMapper(_ => { },
            typeof(CustomerSupportCRM.Application.DependencyInjection).Assembly,
            typeof(DependencyInjection).Assembly);

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

            options.AddInterceptors(sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>());
        });

        services.AddIdentityInfrastructure();

        services.AddScoped<IEmailSender, LoggingEmailSender>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<Application.Features.Roles.IRoleAdminService, RoleAdminService>();
        services.AddScoped<Application.Features.RolePermissions.IRolePermissionAdminService, RolePermissionAdminService>();
        services.AddScoped<IAuditLogService, AuditLogService>();

        services.AddOptions<FileStorageSettings>()
            .Bind(configuration.GetSection(FileStorageSettings.SectionName));
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IEmailVerificationSender, NoopEmailVerificationSender>();

        // F03 email-communication-channel.
        services.AddOptions<EmailSettings>()
            .Bind(configuration.GetSection(EmailSettings.SectionName));
        services.AddScoped<ITicketEmailSender, SmtpTicketEmailSender>();
        services.AddScoped<IInboundEmailProcessor, InboundEmailProcessor>();

        return services;
    }
}
