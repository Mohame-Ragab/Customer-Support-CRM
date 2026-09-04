using CustomerSupportCRM.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerSupportCRM.API.Extensions;

/// <summary>Health checks for architecture verification: application liveness plus SQL Server/<see cref="ApplicationDbContext"/> connectivity.</summary>
public static class HealthCheckServiceExtensions
{
    public static IServiceCollection AddHealthCheckServices(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>("database");

        return services;
    }

    public static WebApplication MapHealthCheckEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health");
        return app;
    }
}
