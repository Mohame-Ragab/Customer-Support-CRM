using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.CustomerPortal.Tickets;

/// <summary>
/// F08 customer-portal/submit-ticket-via-customer-portal. No CustomerId field -
/// the caller's own identity is resolved server-side from the JWT (via
/// ICustomerResolver, matching customer email against the Customer CRM
/// record - same pattern as F03 live-chat), never accepted from the client.
/// </summary>
public sealed record SubmitPortalTicketCommand(string Subject, string Description) : IRequest<TicketDto>;
