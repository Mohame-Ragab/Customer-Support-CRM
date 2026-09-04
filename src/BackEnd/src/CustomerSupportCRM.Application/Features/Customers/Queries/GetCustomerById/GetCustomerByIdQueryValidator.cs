using FluentValidation;

namespace CustomerSupportCRM.Application.Features.Customers.Queries.GetCustomerById;

public sealed class GetCustomerByIdQueryValidator : AbstractValidator<GetCustomerByIdQuery>
{
    public GetCustomerByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
