namespace CustomerSupportCRM.Application.Features.Users.DTOs;

/// <summary>Admin-facing view of a staff account (see security-admin/manage-users). Distinct from <see cref="UserProfileDto"/>, which is the self-service "/me" shape.</summary>
public sealed record UserDto(
    Guid Id,
    string? FullName,
    string Email,
    string Role,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? ModifiedAt);
