using CustomerSupportCRM.Application.Features.RolePermissions.Dtos;
using MediatR;

namespace CustomerSupportCRM.Application.Features.RolePermissions.Queries;

public sealed record GetRolePermissionsQuery(Guid RoleId) : IRequest<RolePermissionsDto>;
