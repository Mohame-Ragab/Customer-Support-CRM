using CustomerSupportCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupportCRM.Infrastructure.Persistence.Configurations;

public sealed class TicketMessageConfiguration : IEntityTypeConfiguration<TicketMessage>
{
    public void Configure(EntityTypeBuilder<TicketMessage> builder)
    {
        builder.ToTable("TicketMessages");

        builder.Property(m => m.Channel).IsRequired().HasConversion<int>();
        builder.Property(m => m.Direction).IsRequired().HasConversion<int>();
        builder.Property(m => m.DeliveryStatus).IsRequired().HasConversion<int>();

        builder.Property(m => m.FromAddress).IsRequired().HasMaxLength(320);
        builder.Property(m => m.ToAddress).IsRequired().HasMaxLength(320);
        builder.Property(m => m.Cc).HasMaxLength(320);
        builder.Property(m => m.Subject).IsRequired().HasMaxLength(200);
        builder.Property(m => m.BodyText).IsRequired().HasColumnType("nvarchar(max)");
        builder.Property(m => m.BodyHtml).HasColumnType("nvarchar(max)");
        builder.Property(m => m.ProviderMessageId).HasMaxLength(400);
        builder.Property(m => m.InReplyToMessageId).HasMaxLength(400);
        builder.Property(m => m.ConversationToken).HasMaxLength(64);
        builder.Property(m => m.FailureReason).HasMaxLength(1000);

        builder.HasIndex(m => m.TicketId);
        builder.HasIndex(m => m.ProviderMessageId);
        builder.HasIndex(m => m.ConversationToken);

        builder.HasOne(m => m.Ticket)
            .WithMany()
            .HasForeignKey(m => m.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
