using CustomerSupportCRM.Application.Features.Agents.Tasks.Dtos;
using FluentValidation;

namespace CustomerSupportCRM.Application.Features.Agents.Tasks.Validators;

public sealed class CreateAgentTaskRequestValidator : AbstractValidator<CreateAgentTaskRequest>
{
    public CreateAgentTaskRequestValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.DueAt).NotEmpty();
    }
}

public sealed class UpdateAgentTaskRequestValidator : AbstractValidator<UpdateAgentTaskRequest>
{
    public UpdateAgentTaskRequestValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.DueAt).NotEmpty();
    }
}
