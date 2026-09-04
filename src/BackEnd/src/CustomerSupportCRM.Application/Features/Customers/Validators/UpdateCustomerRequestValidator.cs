using CustomerSupportCRM.Application.Features.Customers.Dtos;
using FluentValidation;

namespace CustomerSupportCRM.Application.Features.Customers.Validators;

/// <summary>
/// Validates the PUT /api/customers/{id} request body directly, since that is
/// the type ValidationFilter (API layer) actually binds and validates for
/// that action - the id comes from the route, not the body.
/// </summary>
public sealed class UpdateCustomerRequestValidator : AbstractValidator<UpdateCustomerRequest>
{
    public UpdateCustomerRequestValidator()
    {
        this.ApplyCustomerFieldRules();
    }
}
