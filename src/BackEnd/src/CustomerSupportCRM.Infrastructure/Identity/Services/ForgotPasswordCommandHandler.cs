using System.Text;
using System.Web;
using CustomerSupportCRM.Application.Common.Interfaces;
using CustomerSupportCRM.Application.Common.Models;
using CustomerSupportCRM.Application.Features.Users.Commands.ForgotPassword;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CustomerSupportCRM.Infrastructure.Identity.Services;

public sealed class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
{
    private const string GenericMessage = "If an account exists for that email, a password reset link has been sent.";
    private const string EmailSubject = "Reset your password";
    private const string EmailBodyTemplate = "<p>Click <a href=\"{0}\">here</a> to reset your password. This link expires in 60 minutes.</p>";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;
    private readonly PasswordResetSettings _settings;

    public ForgotPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        ILogger<ForgotPasswordCommandHandler> logger,
        IOptions<PasswordResetSettings> settings)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task<ForgotPasswordResponse> Handle(ForgotPasswordCommand command, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(command.Email);

        if (user == null || !user.EmailConfirmed)
        {
            _logger.LogInformation("Forgot password requested for non-existent or unconfirmed email: {Email}", command.Email);
            return new ForgotPasswordResponse(GenericMessage);
        }

        try
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var resetUrl = $"{_settings.ResetUrlBase}?email={HttpUtility.UrlEncode(user.Email!)}&token={encodedToken}";
            var htmlBody = string.Format(EmailBodyTemplate, resetUrl);

            await _emailSender.SendAsync(user.Email!, EmailSubject, htmlBody, ct);

            _logger.LogInformation("Password reset email sent to user: {UserId}", user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email for user: {UserId}", user.Id);
        }

        return new ForgotPasswordResponse(GenericMessage);
    }
}
