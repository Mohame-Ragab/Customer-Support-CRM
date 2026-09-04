using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.Queries.GetTickets;

/// <summary>
/// <paramref name="AssignedToMe"/> (F04 agent-dashboard/view-assigned-tickets):
/// server-derived from <c>ICurrentUserService.UserId</c> in the handler, never
/// from a client-supplied agent id - a caller can only ever filter to their own
/// queue, not anyone else's.
/// </summary>
public sealed record GetTicketsQuery(int Page = 1, int PageSize = 20, bool AssignedToMe = false)
    : IRequest<PagedResult<TicketDto>>;
