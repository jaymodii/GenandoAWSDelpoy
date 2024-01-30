using BusinessAccessLayer.Abstraction;
using Common.Constants;
using Common.Exceptions;
using Common.Utils;
using DataAccessLayer.Abstraction;
using Entities.DataModels;
using Entities.DTOs.Request;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace BusinessAccessLayer.Implementation;

public class AuthenticationService : IAuthenticationService
{

    #region Properties
    private readonly IJwtManageService _jwtManageService;
    private readonly IMailService _mailService;
    public readonly IAuthenticationRepository _authenticationRepository;
    public readonly IUnitOfWork _unitOfWork;
    public readonly IProfileRepository _profileRepository;


    #endregion Properties

    #region Constructor
    public AuthenticationService(IAuthenticationRepository authenticationRepository,
        IMailService mailService, IUnitOfWork unitOfWork, IJwtManageService jwtManageService, IProfileRepository profileRepository)
    {
        _authenticationRepository = authenticationRepository;
        _mailService = mailService;
        _unitOfWork = unitOfWork;
        _jwtManageService = jwtManageService;
        _profileRepository = profileRepository;
    }
    #endregion Constructor

    #region Interface method
    public async Task<string> Login(LoginCredentialDto userCredential)
    {
        User user = await _authenticationRepository.GetUserByEmail(userCredential.Email);
        if (user == null || !PasswordUtil.VerifyPassword(userCredential.Password, user.Password)) throw new ModelValidationException(MessageConstants.InvalidLoginCredential);

        await SendOtp(null, user.Email, SystemConstants.AuthenticationOtp);
        return user.FirstName;
    }

    public async Task<TokensDto> VerifyOtp(long? id, LoginOtpDto otpData, bool rememberMe)
    {
        User user = (id.HasValue
            ? await _authenticationRepository.GetByIdAsync(id.Value)
            : await _authenticationRepository.GetUserByEmail(otpData.Email)) ?? throw new ModelValidationException(MessageConstants.DEFAULT_MODELSTATE);
        if (user.OTP != otpData.Otp || user.ExpiryTime < DateTime.Now) throw new ModelValidationException(MessageConstants.Invalidotp);

        user.OTP = null;
        user.ExpiryTime = null;
        await _authenticationRepository.UpdateAsync(user);
        await _unitOfWork.SaveAsync();

        TokensDto token = _jwtManageService.GenerateToken(user) ?? throw new ModelValidationException(MessageConstants.INVALID_ATTEMPT);
        if (rememberMe)
        {
            UserRefreshTokens userRefreshTokens = new()
            {
                RefreshToken = token.RefreshToken,
                Email = user.Email,
            };
            await _authenticationRepository.AddUserRefreshToken(userRefreshTokens);
            await _unitOfWork.SaveAsync();
        }
        return token;
    }

    public async Task<TokensDto> RefreshToken(TokensDto token)
    {
        ClaimsPrincipal principal = _jwtManageService.GetPrincipalFormExpiredToken(token.AccessToken);
        string email = principal.FindFirstValue(ClaimTypes.Email);
        UserRefreshTokens savedRefreshToken = await _authenticationRepository.GetUserRefreshTokens(email, token.RefreshToken);
        TokensDto newJwtToken = _jwtManageService.GenerateRefreshToken(await _authenticationRepository.GetUserByEmail(email));
        if (savedRefreshToken.RefreshToken != token.RefreshToken || newJwtToken == null) throw new ModelValidationException(MessageConstants.INVALID_ATTEMPT);

        UserRefreshTokens userRefreshTokens = new()
        {
            RefreshToken = newJwtToken.RefreshToken,
            Email = email,
        };
        await _authenticationRepository.DeleteUserRefreshToken(email, token.RefreshToken);
        await _authenticationRepository.AddUserRefreshToken(userRefreshTokens);
        await _unitOfWork.SaveAsync();
        return newJwtToken;
    }

    public async Task SendOtp(long? id, string email, string typeOfOtp)
    {
        User user = id.HasValue
            ? await _authenticationRepository.GetByIdAsync(id.Value)
            : await _authenticationRepository.GetUserByEmail(email);

        if (id.HasValue && await _profileRepository.IsDuplicateEmail(email, id))
            throw new ModelValidationException(MessageConstants.EmailAlreadyExists);

        Random generator = new();
        user.OTP = generator.Next(100000, 999999).ToString();
        user.ExpiryTime = DateTime.Now.AddMinutes(10);

        await _authenticationRepository.UpdateAsync(user);
        await _unitOfWork.SaveAsync();

        string emailBody = typeOfOtp == SystemConstants.AuthenticationOtp
            ? MailBodyUtil.SendOtpForAuthenticationBody(user.OTP)
            : MailBodyUtil.SendOtpForProfileBody(user.OTP);

        await _mailService.SendMailAsync(new MailDto
        {
            ToEmail = email,
            Subject = MailConstants.OtpSubject,
            Body = emailBody
        }); 
    }

    public async Task ForgotPassword(string email)
    {
        if (await _authenticationRepository.GetUserByEmail(email) != null)
        {
            //sent otp in mail
            MailDto mailDto = new()
            {
                ToEmail = email,
                Subject = MailConstants.ResetPasswordSubject,
                Body = MailBodyUtil.SendResetPasswordLink("http://localhost:4200/reset-password?token=" + EncodingMailToken(email))
            };
            await _mailService.SendMailAsync(mailDto);
        }
    }

    public async Task ResetPassword(string password, string token)
    {
        if (String.IsNullOrEmpty(token)) throw new ModelValidationException(MessageConstants.INVALID_TOKEN);
        DateTime dateTime = Convert.ToDateTime(DecodingMailToken(token).Split("&")[1]);
        if (dateTime < DateTime.UtcNow) throw new ModelValidationException(MessageConstants.TOKEN_EXPIRE);

        User user = await _authenticationRepository.GetUserByEmail(DecodingMailToken(token).Split("&")[0]);
        user.Password = PasswordUtil.HashPassword(password);
        await _authenticationRepository.UpdateAsync(user);
        await _unitOfWork.SaveAsync();
    }
    #endregion Interface method

    #region HelperMethod

    public static string EncodingMailToken(string email) => System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(email + "&" + DateTime.UtcNow.AddMinutes(10)));
    public static string DecodingMailToken(string token) => System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(token));
    #endregion HelperMethod
}
