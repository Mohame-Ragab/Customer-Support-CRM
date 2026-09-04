using CustomerSupportCRM.Application.Features.Customers.Notes.Dtos;
using FluentValidation;

namespace CustomerSupportCRM.Application.Features.Customers.Notes.Validators;

public sealed class CreateCustomerNoteRequestValidator : AbstractValidator<CreateCustomerNoteRequest>
{
    public CreateCustomerNoteRequestValidator()
    {
        RuleFor(x => x.Content).NotEmpty().MaximumLength(4000);
    }
}
