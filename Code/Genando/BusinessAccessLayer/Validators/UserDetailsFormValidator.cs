using BusinessAccessLayer.Abstraction;
using Common.Constants;
using Common.Exceptions;
using Common.Utils;
using Entities.DTOs.Request;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using static Common.Constants.ValidationConstants;

namespace BusinessAccessLayer.Validators;
public class UserDetailsFormValidator
    : AbstractValidator<UserDetailsFormRequestDto>
{
    private readonly IUserService _userService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private long? _userId;
    public UserDetailsFormValidator(IUserService userService,
        IHttpContextAccessor httpContextAccessor)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;

        SetRouteId();

        RuleFor(model => model.FirstName)
            .NotEmpty().NotNull()
            .MinimumLength(MinNameLength)
            .MaximumLength(MaxNameLength)
            .Matches(NameRegex)
                .WithMessage("{PropertyName} " + MessageConstants.NameRegExFailed);

        RuleFor(model => model.LastName)
            .NotEmpty().NotNull()
            .MinimumLength(MinNameLength)
            .MaximumLength(MaxNameLength)
            .Matches(NameRegex)
                .WithMessage("{PropertyName} " + MessageConstants.NameRegExFailed);

        RuleFor(model => model.Email)
            .NotEmpty().NotNull()
            .MinimumLength(MinEmailLength)
            .MaximumLength(MaxEmailLength)
            .Matches(EmailRegEx)
                .WithMessage(MessageConstants.EmailRegExFailed)
            .Must(BeAUniqueEmail)
                .WithMessage(MessageConstants.EmailAlreadyExists);

        RuleFor(model => model.PhoneNumber)
            .NotEmpty().NotNull()
            .Must(phoneNumber => phoneNumber.Length == PhoneNumberLength)
                .WithMessage(MessageConstants.PhoneNumberLength)
            .Matches(PhoneNumberRegEx)
                .WithMessage(MessageConstants.PhoneNumberRegExFailed);

        RuleFor(model => model.Address)
            .NotNull().NotEmpty()
            .MinimumLength(MinAddressLength)
            .MaximumLength(MaxAddressLength);

        When(model => model.IsPatient, () =>
        {
            RuleFor(model => model.DateOfBirth)
                .NotNull().NotEmpty()
                .LessThan(DateUtil.UtcNow)
                    .WithMessage("{PropertyName}" + MessageConstants.LessThanCurrentDate);

            RuleFor(model => model.Gender)
                .NotNull().NotEmpty()
                .IsInEnum()
                    .WithMessage("Invalid {PropertyName}");
        });
    }

    private bool BeAUniqueEmail(string email)
        => !(_userService.IsDuplicateEmail(email, _userId).Result);

    private void SetRouteId()
    {
        var routeParam = _httpContextAccessor.HttpContext?.Request.RouteValues["id"]?.ToString();

        _userId = routeParam is null ? null : long.Parse(routeParam);

        if (_userId is not null && _userId <= 0)
            throw new ModelValidationException(MessageConstants.InvalidId.Replace("#", "User"));
    }
}
