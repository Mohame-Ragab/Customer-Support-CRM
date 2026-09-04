using CustomerSupportCRM.Application.Features.Customers.Validators;
using FluentValidation;

namespace CustomerSupportCRM.Application.Features.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        this.ApplyCustomerFieldRules();
    }
}
