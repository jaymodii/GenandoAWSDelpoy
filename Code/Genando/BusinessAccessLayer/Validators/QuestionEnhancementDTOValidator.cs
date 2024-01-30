using Common.Constants;
using Entities.DTOs.Request;
using Entities.DTOs.Response;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAccessLayer.Validators;

public class QuestionEnhancementDTOValidator: AbstractValidator<QuestionEnhancementDTO>
{
    public QuestionEnhancementDTOValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(a => a.Question).NotNull().NotEmpty()
            .Length(ValidationConstants.MinQuestionLength, ValidationConstants.MaxQuestionLength);

        RuleFor(a => a.TypeOfQuestion).NotNull().NotEmpty();
    }
}

public class ClinicalAnswerRequestDTOValidator : AbstractValidator<ClinicalAnswerRequestDTO>
{
    public ClinicalAnswerRequestDTOValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(a => a.PatientId).NotNull().NotEmpty();
    }
}


