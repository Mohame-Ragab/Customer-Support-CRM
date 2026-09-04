using System.Reflection;
using CustomerSupportCRM.Application.Common.Models;
using Microsoft.OpenApi;

namespace CustomerSupportCRM.API.Extensions;

/// <summary>Swagger/OpenAPI setup, including the JWT Bearer security scheme and XML-comment-based documentation.</summary>
public static class SwaggerServiceExtensions
{
    private const string BearerScheme = "Bearer";

    public static IServiceCollection AddSwaggerDocumentation(
        this IServiceCollection services, IConfiguration configuration)
    {
        var applicationName = configuration[$"{ApplicationSettings.SectionName}:{nameof(ApplicationSettings.ApplicationName)}"];
        var title = string.IsNullOrWhiteSpace(applicationName)
            ? new ApplicationSettings().ApplicationName
            : applicationName;

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = $"{title} API",
                Version = "v1",
                Description = "RESTful API for the Customer Support CRM.",
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            options.AddSecurityDefinition(BearerScheme, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter a valid JWT access token.",
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(BearerScheme, document)] = [],
            });
        });

        return services;
    }

    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer Support CRM API v1");
        });

        return app;
    }
}
