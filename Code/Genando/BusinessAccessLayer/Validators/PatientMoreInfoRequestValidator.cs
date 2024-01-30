using BusinessAccessLayer.Profiles;
using Common.Constants;
using Common.Enums;
using Entities.DTOs.Response;
using FluentValidation;

namespace BusinessAccessLayer.Validators;

public class PatientMoreInfoRequestValidator
    : AbstractValidator<PatientMoreInfoRequestDto>
{
    public PatientMoreInfoRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleForEach(model => model.PatientMoreInfo)
            .SetValidator(new PatientMoreInfoResponseValidator());

        RuleFor(model => model.Status)
            .IsInEnum()
                .WithMessage(MessageConstants.PatientQuestionStatus)
            .When(dto => dto.Status != PatientQuestionStatusType.Deleted)
                .WithMessage(MessageConstants.PatientQuestionStatus);

        When(model => model.Status == PatientQuestionStatusType.PublishedByPatient,
            () =>
            {
                RuleFor(model => model.PatientMoreInfo)
                    .Must(answers
                        => answers.All(answer => !string.IsNullOrEmpty(answer.Answer)))
                        .WithErrorCode("Patient Answer")
                        .WithMessage(MessageConstants.PublishMoreInfoError);
            });
        When(model => model.Status == PatientQuestionStatusType.DraftByPatient,
            () =>
            {
                RuleFor(model => model.PatientMoreInfo)
                    .Must(answers
                        => answers.Any(answer => !string.IsNullOrEmpty(answer.Answer)))
                        .WithErrorCode("Patient Answer")
                        .WithMessage(MessageConstants.DraftMoreInfoError);
            });
    }
}