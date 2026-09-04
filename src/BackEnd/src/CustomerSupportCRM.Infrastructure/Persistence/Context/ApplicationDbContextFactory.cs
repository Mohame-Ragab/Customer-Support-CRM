using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CustomerSupportCRM.Infrastructure.Persistence.Context;

/// <summary>
/// Enables `dotnet ef migrations` / `dotnet ef database update` to create
/// <see cref="ApplicationDbContext"/> at design time without running the full
/// API host (and therefore without needing its DI container or a real, reachable
/// database). The connection string here is only used to select the SQL Server
/// provider/dialect for scaffolding migrations - never for a live connection.
/// </summary>
public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        var connectionString =
            Environment.GetEnvironmentVariable("CUSTOMERSUPPORTCRM_CONNECTIONSTRING")
            ?? "Server=.;Initial Catalog=CustomerSupportDB;Integrated Security=True;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";

        optionsBuilder.UseSqlServer(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
