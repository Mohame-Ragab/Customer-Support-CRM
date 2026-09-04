using CustomerSupportCRM.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupportCRM.Infrastructure.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for <see cref="ApplicationUser"/>. Demonstrates the
/// required <see cref="IEntityTypeConfiguration{TEntity}"/> pattern; entity
/// configuration is never placed on the entity class itself.
/// </summary>
public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Users");

        builder.Property(u => u.FullName).HasMaxLength(100);
        builder.Property(u => u.IsActive).HasDefaultValue(true).IsRequired();
        builder.Property(u => u.CreatedAt).IsRequired();
    }
}
