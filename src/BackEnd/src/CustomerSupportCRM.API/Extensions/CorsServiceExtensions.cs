using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerSupportCRM.API.Extensions;

/// <summary>
/// Configuration-driven CORS. Origins come from <c>Cors:AllowedOrigins</c> in
/// appsettings (per environment) - never a wildcard in production.
/// </summary>
public static class CorsServiceExtensions
{
    public const string PolicyName = "CustomerSupportCRMCorsPolicy";

    public static IServiceCollection AddCorsPolicy(
        this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                if (allowedOrigins.Length > 0)
                {
                    policy.WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
                else
                {
                    // No origins configured (e.g. a fresh environment): deny all
                    // cross-origin requests rather than falling back to AllowAnyOrigin().
                    policy.WithOrigins([]);
                }
            });
        });

        return services;
    }
}
