using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Features.Auth.Commands.RegisterCustomer;
using CustomerSupportCRM.Application.Features.Tickets.CustomerResolution;
using CustomerSupportCRM.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CustomerSupportCRM.Infrastructure.Identity.Services;

public sealed class RegistrationService : IRegistrationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailVerificationSender _emailVerificationSender;
    private readonly ICustomerResolver _customerResolver;
    private readonly ILogger<RegistrationService> _logger;

    public RegistrationService(
        UserManager<ApplicationUser> userManager,
        IEmailVerificationSender emailVerificationSender,
        ICustomerResolver customerResolver,
        ILogger<RegistrationService> logger)
    {
        _userManager = userManager;
        _emailVerificationSender = emailVerificationSender;
        _customerResolver = customerResolver;
        _logger = logger;
    }

    public async Task<(bool Success, RegisterCustomerResponse? Response, string? ErrorKey)> RegisterAsync(
        RegisterCustomerCommand command,
        CancellationToken cancellationToken)
    {
        // Normalize email
        var normalizedEmail = command.Email.Trim().ToLowerInvariant();

        // Check if email already exists
        var existingUser = await _userManager.FindByEmailAsync(normalizedEmail);
        if (existingUser != null)
        {
            return (false, null, "Registration.EmailAlreadyExists");
        }

        // Create user
        var user = new ApplicationUser
        {
            UserName = normalizedEmail,
            Email = normalizedEmail,
            EmailConfirmed = false,
        };

        var createResult = await _userManager.CreateAsync(user, command.Password);
        if (!createResult.Succeeded)
        {
            // Map Identity errors to registration error keys
            var errorCode = createResult.Errors.FirstOrDefault()?.Code ?? "PasswordMismatch";
            var mappedKey = MapIdentityErrorToKey(errorCode);
            return (false, null, mappedKey);
        }

        // Add to Customer role
        var roleResult = await _userManager.AddToRoleAsync(user, Roles.Customer);
        if (!roleResult.Succeeded)
        {
            // Roll back the created user so we don't leave an account with no role assigned.
            await _userManager.DeleteAsync(user);
            return (false, null, "UnexpectedError");
        }

        // Create (or link, or backfill-link) the Customer CRM record for this
        // portal account - see ICustomerResolver. Best-effort: a failure here
        // must not fail registration itself (the account is already created
        // and usable; the link can be established lazily on first portal
        // action instead - same resolver, same fallback path).
        try
        {
            await _customerResolver.ResolveForApplicationUserAsync(
                user.Id, normalizedEmail, user.FullName, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to link a Customer record for newly registered user {UserId}", user.Id);
        }

        // Queue email verification
        await _emailVerificationSender.QueueAsync(user.Id, user.Email, cancellationToken);

        // Return response
        var response = new RegisterCustomerResponse(user.Id, user.Email!);
        return (true, response, null);
    }

    private static string MapIdentityErrorToKey(string identityErrorCode)
    {
        return identityErrorCode switch
        {
            "PasswordTooShort" => "Registration.PasswordTooWeak",
            "PasswordRequiresNonAlphanumeric" => "Registration.PasswordTooWeak",
            "PasswordRequiresDigit" => "Registration.PasswordTooWeak",
            "PasswordRequiresUpper" => "Registration.PasswordTooWeak",
            "PasswordRequiresLower" => "Registration.PasswordTooWeak",
            "DuplicateUserName" => "Registration.EmailAlreadyExists",
            "DuplicateEmail" => "Registration.EmailAlreadyExists",
            _ => "Registration.PasswordTooWeak"
        };
    }
}
