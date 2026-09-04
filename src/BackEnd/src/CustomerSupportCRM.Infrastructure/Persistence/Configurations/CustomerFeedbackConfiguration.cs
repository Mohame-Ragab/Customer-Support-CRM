using CustomerSupportCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupportCRM.Infrastructure.Persistence.Configurations;

public sealed class CustomerFeedbackConfiguration : IEntityTypeConfiguration<CustomerFeedback>
{
    public void Configure(EntityTypeBuilder<CustomerFeedback> builder)
    {
        builder.ToTable("CustomerFeedbacks");

        builder.Property(f => f.Rating).IsRequired();
        builder.Property(f => f.Comment).HasMaxLength(2000);
        builder.Property(f => f.CustomerUserId).IsRequired();

        // Race-safety net alongside the handler's pre-check ExistsAsync call.
        builder.HasIndex(f => new { f.TicketId, f.CustomerUserId }).IsUnique().HasFilter("[IsDeleted] = 0");

        builder.HasOne<Ticket>()
            .WithMany()
            .HasForeignKey(f => f.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
