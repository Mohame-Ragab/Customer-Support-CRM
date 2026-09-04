using CustomerSupportCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupportCRM.Infrastructure.Persistence.Configurations;

public sealed class TicketCategoryConfiguration : IEntityTypeConfiguration<TicketCategory>
{
    private static readonly Guid GeneralId = Guid.Parse("33333333-3333-3333-3333-333333333301");
    private static readonly Guid BillingId = Guid.Parse("33333333-3333-3333-3333-333333333302");
    private static readonly Guid TechnicalId = Guid.Parse("33333333-3333-3333-3333-333333333303");
    private static readonly Guid AccountId = Guid.Parse("33333333-3333-3333-3333-333333333304");
    private static readonly Guid OtherId = Guid.Parse("33333333-3333-3333-3333-333333333305");

    public void Configure(EntityTypeBuilder<TicketCategory> builder)
    {
        builder.ToTable("TicketCategories");

        builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
        builder.Property(x => x.NameEn).HasMaxLength(100).IsRequired();
        builder.Property(x => x.NameAr).HasMaxLength(100).IsRequired();
        builder.Property(x => x.IsActive).HasDefaultValue(true).IsRequired();

        builder.HasIndex(x => x.Code).IsUnique();

        var seedCreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        builder.HasData(
            new TicketCategory { Id = GeneralId, Code = "general", NameEn = "General", NameAr = "عام", IsActive = true, CreatedAt = seedCreatedAt },
            new TicketCategory { Id = BillingId, Code = "billing", NameEn = "Billing", NameAr = "الفواتير", IsActive = true, CreatedAt = seedCreatedAt },
            new TicketCategory { Id = TechnicalId, Code = "technical", NameEn = "Technical", NameAr = "فني", IsActive = true, CreatedAt = seedCreatedAt },
            new TicketCategory { Id = AccountId, Code = "account", NameEn = "Account", NameAr = "الحساب", IsActive = true, CreatedAt = seedCreatedAt },
            new TicketCategory { Id = OtherId, Code = "other", NameEn = "Other", NameAr = "أخرى", IsActive = true, CreatedAt = seedCreatedAt });
    }
}
