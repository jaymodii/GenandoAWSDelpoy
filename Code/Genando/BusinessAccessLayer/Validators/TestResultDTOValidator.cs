using Common.Constants;
using Entities.DTOs.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAccessLayer.Validators
{
    public class TestResultDTOValidator:AbstractValidator<TestResultDTO>
    {
        public TestResultDTOValidator() 
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(a => a.ClinicalProcessTestId)
                 .NotNull().NotEmpty();

            RuleFor(a => a.ExternalLink)
                .Matches(ValidationConstants.LinkRegEx).WithMessage(MessageConstants.LinkRegExFailed);

            RuleFor(a => a.ReportAttachmentTitle)
                 .NotNull().NotEmpty();

            RuleFor(a => a.LabNotes)
                 .NotNull().NotEmpty();

            RuleFor(a => a.ReportAttachment)
                 .NotNull().NotEmpty();
        }
    }
}
