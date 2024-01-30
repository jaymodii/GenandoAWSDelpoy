using Entities.DTOs.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAccessLayer.Validators
{
    public class PrescribeTestRequestDTOValidator:AbstractValidator<PrescribeTestRequestDTO>
    {
        public PrescribeTestRequestDTOValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(a => a.ClinicalProcessId)
                 .NotNull().NotEmpty();

            RuleFor(a => a.TestIds)
                 .NotNull().NotEmpty();
        }
    }
}
