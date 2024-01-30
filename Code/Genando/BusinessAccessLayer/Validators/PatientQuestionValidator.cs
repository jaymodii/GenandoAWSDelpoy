using Common.Constants;
using Entities.DTOs.Request;
using FluentValidation;

namespace BusinessAccessLayer.Validators;

public class PatientQuestionValidator
    : AbstractValidator<PatientQuestionsResponseDto>
{
    public PatientQuestionValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(model => model.Id)
            .NotNull().NotEmpty()
            .When(model => model.Id > 0)
                .WithMessage(MessageConstants.IdValueError);

        //RuleFor(model => model.Status)
        //    .IsInEnum()
        //        .WithMessage(MessageConstants.PatientQuestionStatus)
        //    .When(dto => dto.Status != PatientQuestionStatusType.Deleted)
        //        .WithMessage(MessageConstants.PatientQuestionStatus);

        RuleFor(model => model.Question)
            .NotNull().NotEmpty()
            .MinimumLength(3);
    }
}