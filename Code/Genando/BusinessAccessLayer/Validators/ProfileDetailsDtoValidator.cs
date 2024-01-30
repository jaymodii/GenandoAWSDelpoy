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
    public class ProfileDetailsDtoValidator : AbstractValidator<ProfileDetailsDto>
    {
        public ProfileDetailsDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(a => a.FirstName).NotNull().NotEmpty()
                .Length(ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

            RuleFor(a => a.LastName).NotNull().NotEmpty()
                .Length(ValidationConstants.MinNameLength, ValidationConstants.MaxNameLength);

            RuleFor(a => a.PhoneNumber).NotNull().NotEmpty()
                .Must(phoneNumber => phoneNumber.Length == ValidationConstants.PhoneNumberLength)
                    .WithMessage(MessageConstants.PhoneNumberLength)
                .Matches(ValidationConstants.PhoneNumberRegEx)
                    .WithMessage(MessageConstants.PhoneNumberRegExFailed);

            RuleFor(a => a.Email)
                .NotNull().NotEmpty()
                .Matches(ValidationConstants.EmailRegEx).WithMessage(MessageConstants.EmailRegExFailed)
                .Length(ValidationConstants.MinEmailLength, ValidationConstants.MaxEmailLength);

            RuleFor(a => a.Address).NotEmpty().NotNull()
                .Length(ValidationConstants.MinAddressLength, ValidationConstants.MaxAddressLength);
                
        }
    }
}
