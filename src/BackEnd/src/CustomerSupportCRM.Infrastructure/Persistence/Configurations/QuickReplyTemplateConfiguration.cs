using CustomerSupportCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupportCRM.Infrastructure.Persistence.Configurations;

public sealed class QuickReplyTemplateConfiguration : IEntityTypeConfiguration<QuickReplyTemplate>
{
    public void Configure(EntityTypeBuilder<QuickReplyTemplate> builder)
    {
        builder.ToTable("QuickReplyTemplates");

        builder.Property(t => t.OwnerUserId).IsRequired();
        builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Body).IsRequired().HasMaxLength(4000);

        // Filtered so a soft-deleted template's name can be reused.
        builder.HasIndex(t => new { t.OwnerUserId, t.Name }).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasIndex(t => t.OwnerUserId);
    }
}
