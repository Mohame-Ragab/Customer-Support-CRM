using FluentValidation.Results;

namespace CustomerSupportCRM.Application.Common.Exceptions;

/// <summary>
/// Thrown when one or more FluentValidation rules fail. Lives in Application
/// (rather than Domain) because it depends on FluentValidation's
/// <see cref="ValidationFailure"/> type. Caught by the API's global exception
/// handler and translated into an RFC 7807 validation <c>ProblemDetails</c>
/// response (HTTP 400) with per-property error messages.
/// </summary>
public sealed class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures) : this()
    {
        Errors = failures
            .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }
}
