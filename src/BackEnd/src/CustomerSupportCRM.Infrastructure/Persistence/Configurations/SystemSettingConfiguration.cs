using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupportCRM.Infrastructure.Persistence.Configurations;

public sealed class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
{
    // Fixed GUIDs so the seed migration is deterministic across environments.
    private static readonly Guid DefaultLanguageId = Guid.Parse("11111111-1111-1111-1111-111111111101");
    private static readonly Guid DefaultPriorityId = Guid.Parse("11111111-1111-1111-1111-111111111102");
    private static readonly Guid AutoCloseAfterDaysId = Guid.Parse("11111111-1111-1111-1111-111111111103");

    public void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        builder.ToTable("SystemSettings");

        builder.Property(x => x.Key).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.Key).IsUnique();

        builder.Property(x => x.Value).HasMaxLength(4000).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.ValueType).HasConversion<int>();

        builder.HasData(
            new SystemSetting
            {
                Id = DefaultLanguageId,
                Key = "Support.DefaultLanguage",
                Value = "en",
                ValueType = SystemSettingValueType.String,
                Description = "Default UI language for new sessions.",
                IsRequired = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new SystemSetting
            {
                Id = DefaultPriorityId,
                Key = "Tickets.DefaultPriority",
                Value = "Medium",
                ValueType = SystemSettingValueType.String,
                Description = "Default priority assigned to newly created tickets.",
                IsRequired = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new SystemSetting
            {
                Id = AutoCloseAfterDaysId,
                Key = "Tickets.AutoCloseAfterDays",
                Value = "14",
                ValueType = SystemSettingValueType.Integer,
                Description = "Days of inactivity before a resolved ticket auto-closes.",
                IsRequired = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            });
    }
}
