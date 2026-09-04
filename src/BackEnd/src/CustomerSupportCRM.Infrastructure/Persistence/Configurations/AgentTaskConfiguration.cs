using CustomerSupportCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupportCRM.Infrastructure.Persistence.Configurations;

public sealed class AgentTaskConfiguration : IEntityTypeConfiguration<AgentTask>
{
    public void Configure(EntityTypeBuilder<AgentTask> builder)
    {
        builder.ToTable("AgentTasks");

        builder.Property(t => t.Description).IsRequired().HasMaxLength(1000);
        builder.Property(t => t.OwnerUserId).IsRequired();
        builder.Property(t => t.DueAt).HasColumnType("datetime2");
        builder.Property(t => t.CompletedAt).HasColumnType("datetime2");

        builder.HasIndex(t => new { t.OwnerUserId, t.DueAt });
        builder.HasIndex(t => new { t.OwnerUserId, t.IsCompleted, t.DueAt });

        // No FK on TicketId - reserved column only (see AgentTask.cs).
    }
}
