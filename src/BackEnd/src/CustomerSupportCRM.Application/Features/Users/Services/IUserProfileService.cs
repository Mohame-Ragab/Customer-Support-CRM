using CustomerSupportCRM.Application.Features.Users.DTOs;

namespace CustomerSupportCRM.Application.Features.Users.Services;

public interface IUserProfileService
{
    Task<UserProfileDto> GetCurrentAsync(CancellationToken cancellationToken);
    Task<UserProfileDto> UpdateCurrentAsync(UpdateUserProfileRequest request, CancellationToken cancellationToken);
}
