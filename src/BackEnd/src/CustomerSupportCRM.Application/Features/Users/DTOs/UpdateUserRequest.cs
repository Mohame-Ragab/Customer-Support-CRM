namespace CustomerSupportCRM.Application.Features.Users.DTOs;

/// <summary>PUT /api/users/{id} request body. The target user's id comes from the route, not this body - see <see cref="Commands.UpdateUser.UpdateUserCommand"/>, which the controller assembles from both.</summary>
public sealed record UpdateUserRequest(string FullName, string Role, bool IsActive);
