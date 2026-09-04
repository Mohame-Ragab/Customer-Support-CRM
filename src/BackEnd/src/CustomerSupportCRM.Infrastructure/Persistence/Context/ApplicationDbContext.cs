using System.Linq.Expressions;
using System.Reflection;
using CustomerSupportCRM.Domain.Common;
using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CustomerSupportCRM.Infrastructure.Persistence.Context;

/// <summary>
/// The single EF Core context for the whole system: Identity tables (Users,
/// Roles, ...) and future CRM aggregate tables share one SQL Server database.
/// See docs/architecture.md, "Identity database integration" for why a separate
/// Identity context was not introduced.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<AuditLogEntry> AuditLogs => Set<AuditLogEntry>();

    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<BrandingSettings> BrandingSettings => Set<BrandingSettings>();

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Ticket> Tickets => Set<Ticket>();

    public DbSet<TicketCategory> TicketCategories => Set<TicketCategory>();

    public DbSet<TicketAssignment> TicketAssignments => Set<TicketAssignment>();

    public DbSet<TicketEscalation> TicketEscalations => Set<TicketEscalation>();

    public DbSet<TicketHistoryEntry> TicketHistoryEntries => Set<TicketHistoryEntry>();

    public DbSet<CustomerNote> CustomerNotes => Set<CustomerNote>();

    public DbSet<CustomerAttachment> CustomerAttachments => Set<CustomerAttachment>();

    public DbSet<TicketMessage> TicketMessages => Set<TicketMessage>();

    public DbSet<ChatSession> ChatSessions => Set<ChatSession>();

    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    public DbSet<AgentTask> AgentTasks => Set<AgentTask>();

    public DbSet<TicketInternalComment> TicketInternalComments => Set<TicketInternalComment>();

    public DbSet<QuickReplyTemplate> QuickReplyTemplates => Set<QuickReplyTemplate>();

    public DbSet<KnowledgeBaseContent> KnowledgeBaseContents => Set<KnowledgeBaseContent>();

    public DbSet<CustomerFeedback> CustomerFeedbacks => Set<CustomerFeedback>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        ApplySoftDeleteQueryFilters(modelBuilder);
    }

    /// <summary>
    /// Applies a global query filter (<c>IsDeleted == false</c>) to every entity
    /// type deriving from <see cref="BaseEntity"/>, via reflection so future
    /// entities are covered automatically without editing this method.
    /// </summary>
    private static void ApplySoftDeleteQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
            var condition = Expression.Equal(property, Expression.Constant(false));
            var lambda = Expression.Lambda(condition, parameter);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }
    }
}
