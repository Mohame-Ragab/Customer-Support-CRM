namespace CustomerSupportCRM.Application.Common.Models;

public class EmailVerificationSettings
{
    public const string SectionName = "EmailVerification";
    public string FrontendConfirmUrl { get; set; } = "http://localhost:5173/verify-email";
    public int TokenLifespanHours { get; set; } = 24;
}
