using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.Users.DTOs;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Users.Queries.ListUsers;

public sealed record ListUsersQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    string? Role = null) : IRequest<PagedResult<UserDto>>;
