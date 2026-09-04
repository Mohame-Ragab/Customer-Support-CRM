using CustomerSupportCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupportCRM.Infrastructure.Persistence.Configurations;

public sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");

        builder.Property(t => t.Subject).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Description).HasMaxLength(4000);
        builder.Property(t => t.Status).IsRequired().HasConversion<int>();
        builder.Property(t => t.CustomerId).IsRequired();
        builder.Property(t => t.Priority).HasConversion<int?>();
        builder.Property(t => t.Channel).IsRequired().HasConversion<int>();

        builder.HasIndex(t => t.CustomerId);
        builder.HasIndex(t => t.AssignedAgentId);

        // Customer (F01) already shipped, so this is a real FK - Restrict
        // (not Cascade) so deleting/soft-deleting a customer never silently
        // removes their ticket history.
        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(t => t.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // tickets/set-ticket-category-and-priority: SetNull so deactivating/
        // removing a category never blocks or cascades into ticket data.
        builder.HasOne(t => t.Category)
            .WithMany()
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
