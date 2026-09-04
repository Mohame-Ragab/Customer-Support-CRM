using CustomerSupportCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupportCRM.Infrastructure.Persistence.Configurations;

public sealed class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
    public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Action).IsRequired().HasMaxLength(128);
        builder.Property(x => x.PerformedByUserId).HasMaxLength(64);
        builder.Property(x => x.PerformedByUserName).HasMaxLength(256);
        builder.Property(x => x.EntityType).HasMaxLength(64);
        builder.Property(x => x.EntityId).HasMaxLength(64);
        builder.Property(x => x.Summary).HasMaxLength(512);
        builder.Property(x => x.MetadataJson).HasColumnType("nvarchar(max)");

        builder.HasIndex(x => x.TimestampUtc);
        builder.HasIndex(x => new { x.Action, x.TimestampUtc });
    }
}
