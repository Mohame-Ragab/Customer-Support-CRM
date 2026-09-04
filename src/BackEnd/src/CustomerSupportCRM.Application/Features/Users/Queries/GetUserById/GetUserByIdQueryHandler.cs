using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Features.Users.DTOs;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserManagementService _userManagementService;

    public GetUserByIdQueryHandler(IUserManagementService userManagementService)
        => _userManagementService = userManagementService;

    public Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        => _userManagementService.GetByIdAsync(request.Id, cancellationToken);
}
