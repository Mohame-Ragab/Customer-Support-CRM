using CustomerSupportCRM.Application.Features.SystemConfiguration.Dtos;
using FluentValidation;

namespace CustomerSupportCRM.Application.Features.SystemConfiguration.Validators;

public sealed class UpdateSystemSettingDtoValidator : AbstractValidator<UpdateSystemSettingDto>
{
    public UpdateSystemSettingDtoValidator()
    {
        // Empty string is allowed here - the precise "required but blank" check
        // happens server-side in SystemConfigurationService, which knows the
        // setting's IsRequired flag; this only guards null/oversized input.
        RuleFor(x => x.Value).NotNull().MaximumLength(4000);
    }
}
