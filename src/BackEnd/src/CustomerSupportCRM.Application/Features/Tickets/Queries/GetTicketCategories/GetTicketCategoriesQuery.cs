using CustomerSupportCRM.Application.Features.Tickets.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Tickets.Queries.GetTicketCategories;

public sealed record GetTicketCategoriesQuery : IRequest<IReadOnlyList<TicketCategoryDto>>;
