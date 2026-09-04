using CustomerSupportCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupportCRM.Infrastructure.Persistence.Configurations;

public sealed class TicketInternalCommentConfiguration : IEntityTypeConfiguration<TicketInternalComment>
{
    public void Configure(EntityTypeBuilder<TicketInternalComment> builder)
    {
        builder.ToTable("TicketInternalComments");

        builder.Property(c => c.Body).IsRequired().HasMaxLength(4000);

        builder.HasIndex(c => new { c.TicketId, c.CreatedAt });

        builder.HasOne<Ticket>()
            .WithMany()
            .HasForeignKey(c => c.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
