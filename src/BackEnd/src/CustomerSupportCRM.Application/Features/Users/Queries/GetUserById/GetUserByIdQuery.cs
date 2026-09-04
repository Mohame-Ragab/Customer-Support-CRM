using CustomerSupportCRM.Application.Features.Users.DTOs;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(Guid Id) : IRequest<UserDto>;
