using CustomerSupportCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupportCRM.Infrastructure.Persistence.Configurations;

public sealed class BrandingSettingsConfiguration : IEntityTypeConfiguration<BrandingSettings>
{
    /// <summary>
    /// Fixed, well-known id for the single branding row. BaseEntity.Id is a
    /// Guid (not an auto-increment int), so "Id == 1" from the plan becomes
    /// this fixed Guid instead - same deterministic-seed pattern used by
    /// SystemSettingConfiguration (security-admin/manage-system-configuration).
    /// </summary>
    public static readonly Guid SingletonId = Guid.Parse("22222222-2222-2222-2222-222222222201");

    public void Configure(EntityTypeBuilder<BrandingSettings> builder)
    {
        builder.ToTable("BrandingSettings");

        builder.Property(x => x.PrimaryColor).HasMaxLength(9).IsRequired();
        builder.Property(x => x.SecondaryColor).HasMaxLength(9).IsRequired();
        builder.Property(x => x.LogoContentType).HasMaxLength(64);
        builder.Property(x => x.LogoBytes).HasColumnType("varbinary(max)");

        builder.HasData(new BrandingSettings
        {
            Id = SingletonId,
            PrimaryColor = "#1565c0",
            SecondaryColor = "#546e7a",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        });
    }
}
