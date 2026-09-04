using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Features.Users.DTOs;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IUserManagementService _userManagementService;

    public UpdateUserCommandHandler(IUserManagementService userManagementService)
        => _userManagementService = userManagementService;

    public Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        => _userManagementService.UpdateAsync(request, cancellationToken);
}
