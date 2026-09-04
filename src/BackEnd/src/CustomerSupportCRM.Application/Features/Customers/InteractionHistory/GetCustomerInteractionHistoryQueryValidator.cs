using FluentValidation;

namespace CustomerSupportCRM.Application.Features.Customers.InteractionHistory;

public sealed class GetCustomerInteractionHistoryQueryValidator : AbstractValidator<GetCustomerInteractionHistoryQuery>
{
    public GetCustomerInteractionHistoryQueryValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
