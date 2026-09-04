using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.CustomerPortal.Tickets;

public sealed record GetMyPortalTicketsQuery(int Page = 1, int PageSize = 20) : IRequest<PagedResult<TicketDto>>;
