using System.Globalization;
using CustomerSupportCRM.Application.Common.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CustomerSupportCRM.API.Localization;

/// <summary>
/// English/Arabic localization setup, driven by the "Localization" configuration
/// section (<see cref="LocalizationSettings"/>) rather than hardcoded values, so
/// an environment can add/change supported cultures without a code change.
/// Selected via the standard <c>Accept-Language</c> header (no query-string/cookie
/// provider — this is a pure REST API).
/// </summary>
public static class LocalizationExtensions
{
    public static IServiceCollection AddApiLocalization(
        this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<LocalizationSettings>()
            .Bind(configuration.GetSection(LocalizationSettings.SectionName));

        services.AddLocalization();
        return services;
    }

    public static IApplicationBuilder UseApiLocalization(this IApplicationBuilder app)
    {
        var settings = app.ApplicationServices.GetRequiredService<IOptions<LocalizationSettings>>().Value;
        var supportedCultures = settings.SupportedCultures.Select(c => new CultureInfo(c)).ToList();

        var options = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(settings.DefaultCulture, settings.DefaultCulture),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures,
        };

        // Accept-Language is honored by default (AcceptLanguageHeaderRequestCultureProvider).
        return app.UseRequestLocalization(options);
    }
}
