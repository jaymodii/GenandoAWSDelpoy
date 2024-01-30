    using Entities.DTOs.Request;
using FluentValidation;

namespace BusinessAccessLayer.Validators;

public class PatientPageRequestValidator
    : AbstractValidator<PatientPageRequestDto>
{
    public PatientPageRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(model => model.Gender)
            .IsInEnum()
                .WithMessage("Invalid {PropertyName}");

        RuleFor(model => model.Status)
            .IsInEnum()
                .WithMessage("Invalid {PropertyName}");

    }
}