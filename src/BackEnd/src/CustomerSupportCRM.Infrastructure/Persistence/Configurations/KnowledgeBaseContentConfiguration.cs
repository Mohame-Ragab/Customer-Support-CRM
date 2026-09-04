using CustomerSupportCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupportCRM.Infrastructure.Persistence.Configurations;

public sealed class KnowledgeBaseContentConfiguration : IEntityTypeConfiguration<KnowledgeBaseContent>
{
    public void Configure(EntityTypeBuilder<KnowledgeBaseContent> builder)
    {
        builder.ToTable("KnowledgeBaseContents");

        builder.Property(c => c.Title).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Body).IsRequired().HasColumnType("nvarchar(max)");
        builder.Property(c => c.Summary).HasMaxLength(500);
        builder.Property(c => c.Type).IsRequired().HasConversion<int>();
        builder.Property(c => c.Language).IsRequired().HasMaxLength(8).HasDefaultValue("en");
        builder.Property(c => c.IsPublished).IsRequired().HasDefaultValue(true);

        builder.HasIndex(c => c.Type);
        builder.HasIndex(c => c.IsPublished);
        builder.HasIndex(c => new { c.Type, c.Language, c.IsPublished });
    }
}
