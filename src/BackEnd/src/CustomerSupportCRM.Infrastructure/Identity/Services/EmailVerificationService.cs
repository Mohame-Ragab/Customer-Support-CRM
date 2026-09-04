using System.Web;
using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CustomerSupportCRM.Infrastructure.Identity.Services;

public sealed class EmailVerificationService : IEmailVerificationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly EmailVerificationSettings _settings;

    public EmailVerificationService(
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        IOptions<EmailVerificationSettings> settings)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _settings = settings.Value;
    }

    public async Task<Result> ConfirmAsync(Guid userId, string token, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result.Failure("Auth.Verification.InvalidOrExpired");
        }

        if (user.EmailConfirmed)
        {
            return Result.Success();
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            return Result.Failure("Auth.Verification.InvalidOrExpired");
        }

        return Result.Success();
    }

    public async Task<Result> ResendAsync(string email, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || user.EmailConfirmed)
        {
            return Result.Success();
        }

        await _userManager.UpdateSecurityStampAsync(user);
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmUrl = $"{_settings.FrontendConfirmUrl}?userId={user.Id}&token={HttpUtility.UrlEncode(token)}";

        // Use hardcoded English strings; controller localizes API responses
        const string subject = "Verify your email";
        var htmlBody = $"<p>Click <a href=\"{confirmUrl}\">here</a> to verify your email address.</p>";

        await _emailSender.SendAsync(user.Email!, subject, htmlBody, ct);
        return Result.Success();
    }
}
