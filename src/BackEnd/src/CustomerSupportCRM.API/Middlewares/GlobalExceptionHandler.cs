using CustomerSupportCRM.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using ApplicationValidationException = CustomerSupportCRM.Application.Common.Exceptions.ValidationException;
using SharedResource = CustomerSupportCRM.API.Resources.SharedResource;

namespace CustomerSupportCRM.API.Middlewares;

/// <summary>
/// Centralized exception handling (ASP.NET Core's <see cref="IExceptionHandler"/>
/// extensibility point, registered via <c>AddExceptionHandler</c> +
/// <c>UseExceptionHandler</c> in Program.cs). Translates known exception types to
/// an RFC 7807 <see cref="ProblemDetails"/> response with the matching HTTP
/// status; anything unexpected is logged server-side and returned as a generic
/// 500 with no internal details (stack traces, connection strings, etc.) leaked
/// to the client.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger, IStringLocalizer<SharedResource> localizer)
    {
        _logger = logger;
        _localizer = localizer;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title) = MapException(exception);

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception processing {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);
        }
        else
        {
            _logger.LogWarning(exception, "Handled exception ({StatusCode}) processing {Method} {Path}",
                statusCode, httpContext.Request.Method, httpContext.Request.Path);
        }

        httpContext.Response.StatusCode = statusCode;

        if (exception is ApplicationValidationException validationException)
        {
            var problemDetails = new ValidationProblemDetails(validationException.Errors)
            {
                Status = statusCode,
                Title = title,
                Instance = httpContext.Request.Path,
            };
            AddTraceId(problemDetails, httpContext);

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = statusCode == StatusCodes.Status500InternalServerError
                ? _localizer[nameof(SharedResourceKeys.UnexpectedError)]
                : exception.Message,
            Instance = httpContext.Request.Path,
        };
        AddTraceId(problem, httpContext);

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

    private (int StatusCode, string Title) MapException(Exception exception) => exception switch
    {
        ApplicationValidationException => (StatusCodes.Status400BadRequest, _localizer[nameof(SharedResourceKeys.ValidationFailed)].Value),
        NotFoundException => (StatusCodes.Status404NotFound, _localizer[nameof(SharedResourceKeys.ResourceNotFound)].Value),
        UnauthorizedException => (StatusCodes.Status401Unauthorized, _localizer[nameof(SharedResourceKeys.UnauthorizedAccess)].Value),
        ForbiddenAccessException => (StatusCodes.Status403Forbidden, _localizer[nameof(SharedResourceKeys.ForbiddenAccess)].Value),
        ConflictException => (StatusCodes.Status409Conflict, _localizer[nameof(SharedResourceKeys.Conflict)].Value),
        // F03 email-communication-channel: SMTP delivery failure. No SharedResource
        // key added - matches the plain-English-message precedent already used for
        // every other F02/F03 domain exception in this codebase (see docs note on
        // Validators/README.md: localized validator/exception messages are
        // aspirational-only and never actually wired up anywhere in this project).
        EmailDeliveryException => (StatusCodes.Status502BadGateway, "Email delivery failed"),
        _ => (StatusCodes.Status500InternalServerError, _localizer[nameof(SharedResourceKeys.UnexpectedError)].Value),
    };

    private static void AddTraceId(ProblemDetails problemDetails, HttpContext httpContext) =>
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
}

/// <summary>Resource key names, kept as constants so they aren't hand-typed (and risk drifting from the .resx files) at each call site.</summary>
internal static class SharedResourceKeys
{
    public const string UnexpectedError = nameof(UnexpectedError);
    public const string ValidationFailed = nameof(ValidationFailed);
    public const string ResourceNotFound = nameof(ResourceNotFound);
    public const string UnauthorizedAccess = nameof(UnauthorizedAccess);
    public const string ForbiddenAccess = nameof(ForbiddenAccess);
    public const string Conflict = nameof(Conflict);
}
