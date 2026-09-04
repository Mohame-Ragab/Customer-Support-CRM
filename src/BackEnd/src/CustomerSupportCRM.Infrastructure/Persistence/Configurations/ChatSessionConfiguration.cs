using CustomerSupportCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupportCRM.Infrastructure.Persistence.Configurations;

public sealed class ChatSessionConfiguration : IEntityTypeConfiguration<ChatSession>
{
    public void Configure(EntityTypeBuilder<ChatSession> builder)
    {
        builder.ToTable("ChatSessions");

        builder.Property(s => s.CustomerUserId).IsRequired();
        builder.Property(s => s.StartedAtUtc).IsRequired();

        builder.HasIndex(s => s.TicketId);
        builder.HasIndex(s => s.CustomerUserId);
        builder.HasIndex(s => s.AssignedAgentUserId);

        builder.HasOne(s => s.Ticket)
            .WithMany()
            .HasForeignKey(s => s.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Messages)
            .WithOne(m => m.Session)
            .HasForeignKey(m => m.ChatSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
