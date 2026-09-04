using AutoMapper;
using CustomerSupportCRM.Application.Common.Exceptions;
using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Features.Users.DTOs;
using CustomerSupportCRM.Application.Features.Users.Services;
using CustomerSupportCRM.Domain.Exceptions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CustomerSupportCRM.Infrastructure.Identity.Services;

public sealed class UserProfileService : IUserProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<UserProfileService> _logger;

    public UserProfileService(
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<UserProfileService> logger)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserProfileDto> GetCurrentAsync(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue || userId.Value == Guid.Empty)
        {
            throw new UnauthorizedException("Current user not found.");
        }

        var user = await _userManager.FindByIdAsync(userId.Value.ToString());
        if (user == null)
        {
            throw new NotFoundException($"User {userId} not found.");
        }

        var dto = _mapper.Map<UserProfileDto>(user);
        var roles = await _userManager.GetRolesAsync(user);
        dto.Roles = (IReadOnlyCollection<string>)roles;

        return dto;
    }

    public async Task<UserProfileDto> UpdateCurrentAsync(UpdateUserProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue || userId.Value == Guid.Empty)
        {
            throw new UnauthorizedException("Current user not found.");
        }

        var user = await _userManager.FindByIdAsync(userId.Value.ToString());
        if (user == null)
        {
            throw new NotFoundException($"User {userId} not found.");
        }

        // Update email if changed
        if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
        {
            var emailResult = await _userManager.SetEmailAsync(user, request.Email);
            if (!emailResult.Succeeded)
            {
                var failures = emailResult.Errors
                    .Select(e => new ValidationFailure(nameof(request.Email), e.Description))
                    .ToList();
                throw new Application.Common.Exceptions.ValidationException(failures);
            }

            // Mark email as unconfirmed
            user.EmailConfirmed = false;
            // TODO: trigger email re-verification (Plan 09)
        }

        // Update phone number if changed
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber) && request.PhoneNumber != user.PhoneNumber)
        {
            var phoneResult = await _userManager.SetPhoneNumberAsync(user, request.PhoneNumber);
            if (!phoneResult.Succeeded)
            {
                var failures = phoneResult.Errors
                    .Select(e => new ValidationFailure(nameof(request.PhoneNumber), e.Description))
                    .ToList();
                throw new Application.Common.Exceptions.ValidationException(failures);
            }
        }

        // Update user
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            _logger.LogError("Failed to update user {UserId}: {Errors}", userId, string.Join(",", updateResult.Errors.Select(e => e.Description)));
            var failures = updateResult.Errors
                .Select(e => new ValidationFailure("General", e.Description))
                .ToList();
            throw new Application.Common.Exceptions.ValidationException(failures);
        }

        var dto = _mapper.Map<UserProfileDto>(user);
        var roles = await _userManager.GetRolesAsync(user);
        dto.Roles = (IReadOnlyCollection<string>)roles;

        _logger.LogInformation("User {UserId} profile updated", userId);
        return dto;
    }
}
