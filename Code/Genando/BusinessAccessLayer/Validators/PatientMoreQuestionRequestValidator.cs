using Common.Constants;
using Common.Enums;
using Entities.DTOs.Request;
using FluentValidation;

namespace BusinessAccessLayer.Validators;

public class PatientMoreQuestionRequestValidator
    : AbstractValidator<PatientMoreQuestionRequestDto>
{
    public PatientMoreQuestionRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleForEach(model => model.Questions)
            .SetValidator(new PatientQuestionValidator());

        RuleFor(model => model.Status)
            .IsInEnum()
                .WithMessage(MessageConstants.PatientQuestionStatus)
            .When(dto => dto.Status != PatientQuestionStatusType.Deleted)
                .WithMessage(MessageConstants.PatientQuestionStatus);
    }
}