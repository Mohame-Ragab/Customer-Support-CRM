using CustomerSupportCRM.Application.Common.Exceptions;
using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.Users.ResetPassword;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CustomerSupportCRM.Infrastructure.Identity.Services;

public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result> Handle(ResetPasswordCommand command, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(command.Email);
        if (user == null)
        {
            _logger.LogWarning("Password reset attempted for non-existent email: {Email}", command.Email);
            return Result.Failure("Auth.ResetPassword.InvalidToken");
        }

        var result = await _userManager.ResetPasswordAsync(user, command.Token, command.NewPassword);

        if (result.Succeeded)
        {
            _logger.LogInformation("Password reset successfully for user: {UserId}", user.Id);
            return Result.Success();
        }

        // Check if any errors are password policy violations
        var passwordErrors = result.Errors
            .Where(e => e.Code.StartsWith("Password"))
            .ToList();

        if (passwordErrors.Any())
        {
            _logger.LogWarning("Password reset failed due to policy violation for user: {UserId}", user.Id);
            var failures = passwordErrors
                .Select(e => new ValidationFailure("NewPassword", e.Description))
                .ToList();
            throw new ValidationException(failures);
        }

        // Invalid or expired token
        _logger.LogWarning("Password reset failed with invalid/expired token for user: {UserId}", user.Id);
        return Result.Failure("Auth.ResetPassword.InvalidToken");
    }
}
