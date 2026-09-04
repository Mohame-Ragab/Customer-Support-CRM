using MediatR;

namespace CustomerSupportCRM.Application.Features.Users.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IRequest<ForgotPasswordResponse>;

public sealed record ForgotPasswordResponse(string Message);
