using CustomerSupportCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupportCRM.Infrastructure.Persistence.Configurations;

public sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");

        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Code).HasMaxLength(20);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.IsActive).HasDefaultValue(true).IsRequired();

        // SQL Server's default collation (SQL_Latin1_General_CP1_CI_AS) is
        // case-insensitive, so a plain unique index gives case-insensitive
        // uniqueness for Name without extra configuration - matches the
        // convention already used for ApplicationUser.Email uniqueness.
        // Filtered to active rows only so a soft-deleted department's name
        // can be reused by a new one.
        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasFilter("[IsActive] = 1")
            .HasDatabaseName("IX_Departments_Name_Active");

        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter("[Code] IS NOT NULL AND [IsActive] = 1")
            .HasDatabaseName("IX_Departments_Code_Active");
    }
}
