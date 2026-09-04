namespace CustomerSupportCRM.Application.Features.Users.DTOs;

public sealed class UpdateUserProfileRequest
{
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}
