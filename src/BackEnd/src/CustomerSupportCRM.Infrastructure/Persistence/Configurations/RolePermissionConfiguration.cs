using CustomerSupportCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupportCRM.Infrastructure.Persistence.Configurations;

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RoleId).IsRequired();

        builder.Property(x => x.PermissionName)
            .IsRequired()
            .HasMaxLength(128);

        builder.HasIndex(x => new { x.RoleId, x.PermissionName }).IsUnique();
    }
}
