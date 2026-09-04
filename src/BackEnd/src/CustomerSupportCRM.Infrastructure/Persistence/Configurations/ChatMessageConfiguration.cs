using CustomerSupportCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupportCRM.Infrastructure.Persistence.Configurations;

public sealed class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("ChatMessages");

        builder.Property(m => m.Kind).IsRequired().HasConversion<int>();
        builder.Property(m => m.Body).IsRequired().HasMaxLength(4000).IsUnicode(true);
        builder.Property(m => m.SentAtUtc).IsRequired();

        builder.HasIndex(m => new { m.ChatSessionId, m.SentAtUtc });
        builder.HasIndex(m => m.TicketId);
    }
}
