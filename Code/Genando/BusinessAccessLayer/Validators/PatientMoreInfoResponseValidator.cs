using Common.Constants;
using Entities.DTOs.Response;
using FluentValidation;

namespace BusinessAccessLayer.Validators;
public class PatientMoreInfoResponseValidator
    : AbstractValidator<PatientMoreInfoResponseDto>
{
    public PatientMoreInfoResponseValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(model => model.Id)
            .NotNull().NotEmpty()
            .When(model => model.Id > 0)
                .WithMessage(MessageConstants.IdValueError);
    }
}
