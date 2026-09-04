using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerSupportCRM.Application;

/// <summary>
/// Composition root entry point for the Application layer. Registers AutoMapper
/// profiles and FluentValidation validators by scanning this assembly, so new
/// features only need to add a <c>Profile</c>/<c>AbstractValidator&lt;T&gt;</c>
/// class - no DI wiring required per feature.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // AutoMapper is registered in the Infrastructure composition root (see
        // AddInfrastructure) instead of here: it must scan both this assembly and
        // Infrastructure's (e.g. UserProfileMappingProfile, which maps the Identity
        // ApplicationUser type Application cannot reference) in a single call, since
        // AddAutoMapper only honors the assemblies passed on its first registration.
        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        services.AddScoped<Features.SystemConfiguration.ISystemConfigurationService, Features.SystemConfiguration.SystemConfigurationService>();
        services.AddScoped<Features.Departments.IDepartmentService, Features.Departments.DepartmentService>();
        services.AddScoped<Features.Branding.IBrandingService, Features.Branding.BrandingService>();
        services.AddScoped<Features.Tickets.History.ITicketHistoryWriter, Features.Tickets.History.TicketHistoryWriter>();
        services.AddScoped<Features.Tickets.CustomerResolution.ICustomerResolver, Features.Tickets.CustomerResolution.CustomerResolver>();

        return services;
    }
}
