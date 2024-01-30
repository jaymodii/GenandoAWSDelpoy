using Common.Constants;
using Entities.DTOs.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAccessLayer.Validators;
public class LoginCredentialDtoValidationRule : AbstractValidator<LoginCredentialDto>
{
    public LoginCredentialDtoValidationRule()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(a => a.Email)
            .NotNull().NotEmpty()
            .Matches(ValidationConstants.EmailRegEx).WithMessage(MessageConstants.EmailRegExFailed)
            .Length(ValidationConstants.MinEmailLength,ValidationConstants.MaxEmailLength);
        RuleFor(a => a.Password)
            .NotNull().NotEmpty()
            .Matches(ValidationConstants.PasswordRegEx).WithMessage(MessageConstants.PasswordRegExFailed)
            .Length(ValidationConstants.MinPasswordLength, ValidationConstants.MaxPasswordLength);
    }
}

public class LoginOtpDtoValidationRule : AbstractValidator<LoginOtpDto>
{
    public LoginOtpDtoValidationRule()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(a => a.Email)
            .NotNull().NotEmpty()
            .Matches(ValidationConstants.EmailRegEx).WithMessage(MessageConstants.EmailRegExFailed)
            .Length(ValidationConstants.MinEmailLength, ValidationConstants.MaxEmailLength);
        RuleFor(a => a.Otp)
            .NotNull().NotEmpty();
    }
}

public class LoginEmailDtoValidationRule : AbstractValidator<LoginEmailDto>
{
    public LoginEmailDtoValidationRule()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(a => a.Email)
            .NotNull().NotEmpty()
            .Matches(ValidationConstants.EmailRegEx).WithMessage(MessageConstants.EmailRegExFailed)
            .Length(ValidationConstants.MinEmailLength, ValidationConstants.MaxEmailLength);
    }
}

public class ResetPasswordDtoValidationRule : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordDtoValidationRule()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(a => a.Password)
           .NotNull().NotEmpty()
           .Matches(ValidationConstants.PasswordRegEx).WithMessage(MessageConstants.PasswordRegExFailed)
           .Length(ValidationConstants.MinPasswordLength, ValidationConstants.MaxPasswordLength);
        RuleFor(a=>a.ConfirmPassword)
            .NotEmpty().NotNull()
            .Equal(p=>p.Password).WithMessage(MessageConstants.PasswordMissMatch);
    }
}

public class TokensDtoValidationRule : AbstractValidator<TokensDto>
{
    public TokensDtoValidationRule()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(a => a.AccessToken)
           .NotNull().NotEmpty();
        RuleFor(a => a.RefreshToken)
           .NotNull().NotEmpty();
    }
}