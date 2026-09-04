namespace CustomerSupportCRM.Application.Features.Auth.Dtos;

public record VerifyEmailRequest(Guid UserId, string Token);
