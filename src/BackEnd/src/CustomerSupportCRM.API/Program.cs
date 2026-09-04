using CustomerSupportCRM.API.Extensions;
using CustomerSupportCRM.API.Hubs;
using CustomerSupportCRM.API.Localization;
using CustomerSupportCRM.API.Middlewares;
using CustomerSupportCRM.API.Services;
using CustomerSupportCRM.Application;
using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Infrastructure;
using CustomerSupportCRM.Infrastructure.Identity;
using CustomerSupportCRM.Infrastructure.Persistence.Context;
using MediatR;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// ---- Services (composition root) -----------------------------------------
// Each layer registers itself through one extension method, keeping this file
// a clean, readable list of what the application is made of.

builder.Services.AddApplication();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(
        typeof(CustomerSupportCRM.Application.DependencyInjection).Assembly,
        typeof(CustomerSupportCRM.Infrastructure.DependencyInjection).Assembly));
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddOptions<ApplicationSettings>()
    .Bind(builder.Configuration.GetSection(ApplicationSettings.SectionName));

builder.Services.AddOptions<PasswordResetSettings>()
    .Bind(builder.Configuration.GetSection(PasswordResetSettings.SectionName));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// F03 live-chat-communication-channel. AgentPresenceTracker is registered
// under both its concrete type (ChatHub needs the mutator methods) and its
// Application-facing interface (IAgentPresenceTracker), as the same singleton
// instance.
builder.Services.AddSingleton<AgentPresenceTracker>();
builder.Services.AddSingleton<IAgentPresenceTracker>(sp => sp.GetRequiredService<AgentPresenceTracker>());
builder.Services.AddScoped<IChatNotifier, ChatNotifier>();
builder.Services.AddChatRealtime();

// F03 web-forms-channel.
builder.Services.AddOptions<WebFormsSettings>()
    .Bind(builder.Configuration.GetSection(WebFormsSettings.SectionName));
builder.Services.AddWebFormsRateLimiting(builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddApiControllers();
builder.Services.AddApiLocalization(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddPermissionAuthorization();
builder.Services.AddCorsPolicy(builder.Configuration);
builder.Services.AddSwaggerDocumentation(builder.Configuration);
builder.Services.AddHealthCheckServices();

var app = builder.Build();

// ---- HTTP request pipeline -------------------------------------------------

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

app.UseApiLocalization();

app.UseHttpsRedirection();

app.UseCors(CorsServiceExtensions.PolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");
app.MapHealthCheckEndpoints();

// Roles only (no users/passwords) - safe to no-op if the database isn't
// reachable yet; see RoleSeeder.
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("RoleSeeder");
    await RoleSeeder.SeedAsync(roleManager, logger);

    // Runs after RoleSeeder so the Admin role already exists; see PermissionSeeder.
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var permissionLogger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("PermissionSeeder");
    await PermissionSeeder.SeedAsync(roleManager, dbContext, permissionLogger);

    // Auth/user-identity gap fix: a known way into a freshly initialized
    // database (see AdminUserSeeder) and a safe, deterministic backfill of
    // Customer.ApplicationUserId for data created before that link existed.
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var adminSeedLogger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("AdminUserSeeder");
    await AdminUserSeeder.SeedAsync(userManager, builder.Configuration, adminSeedLogger);

    var backfillLogger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("CustomerApplicationUserLinkBackfiller");
    await CustomerApplicationUserLinkBackfiller.RunAsync(dbContext, userManager, backfillLogger);
}

app.Run();

/// <summary>Partial <c>Program</c> class so WebApplicationFactory-based integration tests can reference the entry point.</summary>
public partial class Program
{
}
