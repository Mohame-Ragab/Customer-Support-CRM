using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Interfaces;

namespace CustomerSupportCRM.Application.Features.Tickets;

/// <summary>
/// Shared helper so every command/query handler that maps a Ticket to
/// TicketDto gets a populated CategoryCode. Application cannot call EF's
/// Include(), so the category is fetched and attached in-memory instead -
/// used by every F02 handler that returns a TicketDto after a mutation that
/// doesn't already set Ticket.Category itself.
/// </summary>
public static class TicketCategoryAttachment
{
    public static async Task AttachCategoryIfNeededAsync(
        this Ticket ticket, IUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        if (ticket.CategoryId.HasValue && ticket.Category == null)
        {
            ticket.Category = await unitOfWork.Repository<TicketCategory>()
                .GetByIdAsync(ticket.CategoryId.Value, cancellationToken);
        }
    }
}
