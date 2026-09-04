using System.Text.Encodings.Web;
using System.Text.Unicode;
using CustomerSupportCRM.API.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerSupportCRM.API.Extensions;

/// <summary>MVC controller registration, including the JSON serialization settings needed for correct Arabic/English round-tripping.</summary>
public static class ControllerServiceExtensions
{
    public static IServiceCollection AddApiControllers(this IServiceCollection services)
    {
        services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            })
            .AddJsonOptions(options =>
            {
                // System.Text.Json's default encoder escapes every character outside
                // Basic Latin as \uXXXX - which would silently mangle Arabic ("ar")
                // response bodies while still being valid JSON. Allow the full Unicode
                // range through as literal UTF-8 instead.
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

                // Not set explicitly: DateTime always round-trips as ISO-8601 with a
                // "Z" suffix here because every persisted timestamp is DateTime.UtcNow
                // (DateTimeKind.Utc) - see AuditableEntitySaveChangesInterceptor -
                // never server-local time, so no custom converter is needed.
            });

        return services;
    }
}
