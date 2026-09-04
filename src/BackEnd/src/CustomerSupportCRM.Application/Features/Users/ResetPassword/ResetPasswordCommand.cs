using CustomerSupportCRM.Application.Common.Models;
using MediatR;

namespace CustomerSupportCRM.Application.Features.Users.ResetPassword;

public sealed record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword) : IRequest<Result>;
