using System.Threading.RateLimiting;
using CustomerSupportCRM.Application.Common.Models;
using Microsoft.AspNetCore.RateLimiting;

namespace CustomerSupportCRM.API.Extensions;

/// <summary>Anti-abuse rate limiting for the public web-forms intake endpoint (F03 web-forms-channel).</summary>
public static class RateLimitingExtensions
{
    public const string WebFormsPolicyName = "web-forms-submissions";

    public static IServiceCollection AddWebFormsRateLimiting(
        this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection(WebFormsSettings.SectionName).Get<WebFormsSettings>()
            ?? new WebFormsSettings();

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy(WebFormsPolicyName, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = settings.RateLimitPermitPerMinute,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                    }));
        });

        return services;
    }
}
