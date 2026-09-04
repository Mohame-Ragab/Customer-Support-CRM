using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using ValidationException = CustomerSupportCRM.Application.Common.Exceptions.ValidationException;

namespace CustomerSupportCRM.API.Filters;

/// <summary>
/// Integrates FluentValidation into MVC's request pipeline (automatic
/// per-action-argument validation was removed from ASP.NET Core's built-in MVC
/// pipeline, so this replaces the old, now-deprecated
/// <c>FluentValidation.AspNetCore</c> package). For every action argument that
/// has a matching <c>IValidator&lt;T&gt;</c> registered, runs it before the
/// action executes and throws <see cref="ValidationException"/> on failure,
/// which <see cref="Middlewares.GlobalExceptionHandler"/> turns into a 400
/// <c>ProblemDetails</c> response. Registered globally in Program.cs.
/// </summary>
public sealed class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (_serviceProvider.GetService(validatorType) is not IValidator validator)
            {
                continue;
            }

            var validationContext = new ValidationContext<object>(argument);
            var result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }

        await next();
    }
}
