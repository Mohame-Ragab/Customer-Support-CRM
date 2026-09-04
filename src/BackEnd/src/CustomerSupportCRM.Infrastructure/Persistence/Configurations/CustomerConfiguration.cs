using CustomerSupportCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupportCRM.Infrastructure.Persistence.Configurations;

/// <summary>
/// Fluent configuration for <see cref="Customer"/> (customers/create-customer).
/// The unique index on Email is filtered on IsDeleted = 0, matching the global
/// soft-delete query filter - a previously soft-deleted customer's email is
/// therefore reusable by a new customer. This is intentional.
/// </summary>
public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(256).IsRequired();
        builder.Property(x => x.PhoneNumber).HasMaxLength(32);
        builder.Property(x => x.CompanyName).HasMaxLength(200);
        builder.Property(x => x.PreferredLanguage).HasMaxLength(8);

        builder.HasIndex(x => x.Email)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasDatabaseName("IX_Customers_Email_Active");

        // Filtered so most staff-managed Customers (ApplicationUserId null) are
        // excluded - only enforces "at most one Customer per portal account"
        // among the ones that actually have one.
        builder.HasIndex(x => x.ApplicationUserId)
            .IsUnique()
            .HasFilter("[ApplicationUserId] IS NOT NULL AND [IsDeleted] = 0")
            .HasDatabaseName("IX_Customers_ApplicationUserId_Active");
    }
}
