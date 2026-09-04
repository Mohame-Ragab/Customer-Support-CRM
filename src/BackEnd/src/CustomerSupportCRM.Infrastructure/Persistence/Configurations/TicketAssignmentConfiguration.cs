using CustomerSupportCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupportCRM.Infrastructure.Persistence.Configurations;

public sealed class TicketAssignmentConfiguration : IEntityTypeConfiguration<TicketAssignment>
{
    public void Configure(EntityTypeBuilder<TicketAssignment> builder)
    {
        builder.ToTable("TicketAssignments");

        builder.Property(x => x.TicketId).IsRequired();
        builder.Property(x => x.AssignedAgentId).IsRequired();
        builder.Property(x => x.AssignedByUserId).IsRequired();
        builder.Property(x => x.AssignedAtUtc).IsRequired();

        builder.HasIndex(x => new { x.TicketId, x.AssignedAtUtc });

        builder.HasOne<Ticket>()
            .WithMany()
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
