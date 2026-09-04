using System.Text;
using CustomerSupportCRM.Application.Common.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CustomerSupportCRM.API.Extensions;

/// <summary>
/// JWT Bearer authentication + role-based authorization pipeline setup. Only the
/// infrastructure is configured here - no login/register/token-issuing endpoints
/// exist yet (see docs/architecture.md, "Authentication architecture").
/// </summary>
public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException(
                $"Missing required configuration section \"{JwtSettings.SectionName}\".");

        services.AddSingleton(jwtSettings);

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };

                // F03 live-chat-communication-channel: SignalR's browser client
                // cannot set an Authorization header on the WebSocket upgrade
                // request, so it sends the token as a query string parameter
                // instead. Only honored under /hubs, so REST endpoints are
                // unaffected.
                options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },
                };
            });

        // Role-based policies are added per-feature as they become necessary;
        // [Authorize(Roles = "...")] against Domain.Constants.Roles is sufficient for now.
        services.AddAuthorization();

        return services;
    }
}
