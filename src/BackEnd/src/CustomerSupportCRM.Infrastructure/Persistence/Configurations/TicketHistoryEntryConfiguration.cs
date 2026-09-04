using CustomerSupportCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupportCRM.Infrastructure.Persistence.Configurations;

public sealed class TicketHistoryEntryConfiguration : IEntityTypeConfiguration<TicketHistoryEntry>
{
    public void Configure(EntityTypeBuilder<TicketHistoryEntry> builder)
    {
        builder.ToTable("TicketHistoryEntries");

        builder.Property(x => x.TicketId).IsRequired();
        builder.Property(x => x.EventType).IsRequired().HasConversion<int>();
        builder.Property(x => x.OccurredAt).IsRequired();
        builder.Property(x => x.ActorUserId).HasMaxLength(450); // matches Identity user id column width
        builder.Property(x => x.ActorDisplayName).HasMaxLength(256);
        builder.Property(x => x.OldValue).HasMaxLength(512);
        builder.Property(x => x.NewValue).HasMaxLength(512);
        builder.Property(x => x.Note).HasMaxLength(1000);

        builder.HasIndex(x => new { x.TicketId, x.OccurredAt });

        // Tickets table already exists in this codebase (created alongside
        // this same migration), so a real FK is safe. Cascade: the audit
        // trail has no meaning once its parent ticket is gone.
        builder.HasOne<Ticket>()
            .WithMany()
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
