using CustomerSupportCRM.Domain.Entities;
using CustomerSupportCRM.Domain.Exceptions;
using CustomerSupportCRM.Domain.Interfaces;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Customers.InteractionHistory;

/// <summary>
/// Aggregated, newest-first customer interaction timeline
/// (customers/view-customer-interaction-history). Tickets are the interaction
/// source, sorted by <see cref="Ticket.CreatedAt"/> (the only timestamp
/// BaseEntity's audit fields guarantee).
///
/// Bug fix: this handler was originally written before F02 (tickets/create-ticket)
/// existed and shipped as a deliberate no-op stub (always returned an empty
/// list) with a TODO to fill in the real projection once the Ticket entity
/// landed. F02 (and F03, which added Ticket.Channel) have both been
/// implemented since, but the stub was never replaced - every "customer
/// interaction history" response has been empty regardless of how many
/// tickets the customer actually has. Fixed here. The other TODO ("union with
/// CustomerCommunication projections") assumed a standalone communications
/// entity that F03 never introduced - F03 instead added Channel directly onto
/// Ticket, which this projection already surfaces, so no separate union step
/// is needed.
/// </summary>
public sealed class GetCustomerInteractionHistoryQueryHandler
    : IRequestHandler<GetCustomerInteractionHistoryQuery, CustomerInteractionHistoryResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCustomerInteractionHistoryQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<CustomerInteractionHistoryResult> Handle(
        GetCustomerInteractionHistoryQuery request, CancellationToken cancellationToken)
    {
        var customerExists = await _unitOfWork.Repository<Customer>()
            .ExistsAsync(c => c.Id == request.CustomerId, cancellationToken);

        if (!customerExists)
        {
            throw new NotFoundException(nameof(Customer), request.CustomerId);
        }

        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var query = _unitOfWork.Repository<Ticket>().Query()
            .Where(t => t.CustomerId == request.CustomerId);

        var totalCount = query.Count();

        var tickets = query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        IReadOnlyList<CustomerInteractionDto> items = tickets
            .Select(t => new CustomerInteractionDto
            {
                Id = t.Id,
                Type = InteractionType.Ticket,
                OccurredAtUtc = t.CreatedAt,
                Title = t.Subject,
                Summary = t.Description,
                Status = t.Status.ToString(),
                Channel = t.Channel.ToString(),
                SourceEntityId = t.Id,
            })
            .ToList();

        return new CustomerInteractionHistoryResult
        {
            CustomerId = request.CustomerId,
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        };
    }
}
