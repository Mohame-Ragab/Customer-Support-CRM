using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Features.Users.DTOs;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserManagementService _userManagementService;

    public CreateUserCommandHandler(IUserManagementService userManagementService)
        => _userManagementService = userManagementService;

    public Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        => _userManagementService.CreateAsync(request, cancellationToken);
}
