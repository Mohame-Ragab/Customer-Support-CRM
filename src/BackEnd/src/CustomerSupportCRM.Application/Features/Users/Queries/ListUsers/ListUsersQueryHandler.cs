using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.Users.DTOs;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Users.Queries.ListUsers;

public sealed class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, PagedResult<UserDto>>
{
    private readonly IUserManagementService _userManagementService;

    public ListUsersQueryHandler(IUserManagementService userManagementService)
        => _userManagementService = userManagementService;

    public Task<PagedResult<UserDto>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
        => _userManagementService.ListAsync(request, cancellationToken);
}
