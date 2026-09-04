namespace CustomerSupportCRM.Application.Common.Models;

/// <summary>
/// Represents the outcome of an application use case that can fail for
/// business/validation reasons without throwing. Reserved for use cases whose
/// failure is an expected, normal outcome; not a general-purpose wrapper around
/// every response (see docs/architecture.md, "API response strategy").
/// </summary>
public class Result
{
    public bool Succeeded { get; }

    public IReadOnlyList<string> Errors { get; }

    protected Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToList();
    }

    public static Result Success() => new(true, []);

    public static Result Failure(params string[] errors) => new(false, errors);

    public static Result Failure(IEnumerable<string> errors) => new(false, errors);
}

/// <summary>Generic counterpart of <see cref="Result"/> carrying a success value.</summary>
public sealed class Result<T> : Result
{
    public T? Value { get; }

    /// <summary>
    /// For use cases with business/localized errors: a single resource key (e.g., "Auth_InvalidCredentials")
    /// that the API layer can use to fetch a localized error message. Null for non-localized results.
    /// </summary>
    public string? ErrorKey { get; }

    private Result(bool succeeded, T? value, IEnumerable<string> errors, string? errorKey = null)
        : base(succeeded, errors)
    {
        Value = value;
        ErrorKey = errorKey;
    }

    public static Result<T> Success(T value) => new(true, value, []);

    public new static Result<T> Failure(params string[] errors) => new(false, default, errors);

    public new static Result<T> Failure(IEnumerable<string> errors) => new(false, default, errors);

    /// <summary>Failure with a localized error key.</summary>
    public static Result<T> FailureWithKey(string errorKey) => new(false, default, [errorKey], errorKey);
}
