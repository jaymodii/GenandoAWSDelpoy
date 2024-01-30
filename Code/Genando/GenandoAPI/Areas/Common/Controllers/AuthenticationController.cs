using BusinessAccessLayer.Abstraction;
using Common.Constants;
using Entities.DTOs.Request;
using GenandoAPI.Filters;
using GenandoAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Web.Http;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace GenandoAPI.Areas.Common.Controllers;
[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthenticationController : Controller
{
    #region Properties
    public readonly IAuthenticationService _authenticationService;
    #endregion Properties

    #region Constructor
    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }
    #endregion Constructor

    #region Method
    [HttpPost("login")]
    [ValidateModel]
    public async Task<IActionResult> Login(LoginCredentialDto userCredential)
    {
        var cookieHeaderValue = new SetCookieHeaderValue(SystemConstants.RememeberMeCookieKey, userCredential.RememberMe.ToString())
        {
            Expires = DateTime.UtcNow.AddDays(90),
            Path = "/", // Set the cookie path
            Domain = "localhost", // Set the cookie domain
            Secure = true,
            SameSite = Microsoft.Net.Http.Headers.SameSiteMode.None // Set whether the cookie requires a secure connection (https)
        };
        Response.Headers[HeaderNames.SetCookie] = cookieHeaderValue.ToString();
        return ResponseHelper.SuccessResponse(await _authenticationService.Login(userCredential), MessageConstants.MailSent);
    }

    [HttpPost("verify-otp")]
    [ValidateModel]
    public async Task<IActionResult> VerifyOtp(LoginOtpDto otpData)
    {
        bool remeberMe = Request.Cookies[SystemConstants.RememeberMeCookieKey] is not null && Request.Cookies[SystemConstants.RememeberMeCookieKey] == SystemConstants.TrueString;
        return ResponseHelper.SuccessResponse(await _authenticationService.VerifyOtp(null, otpData, remeberMe), MessageConstants.LoginSuccess);
    }

    [HttpPost("refresh-jwttoken")]
    [ValidateModel]
    public async Task<IActionResult> RefreshToken(TokensDto token)
    {
        return ResponseHelper.SuccessResponse(await _authenticationService.RefreshToken(token), string.Empty);
    }

    [HttpPost("resend-otp")]
    [ValidateModel]
    public async Task<IActionResult> ResendOtp(LoginEmailDto emailData)
    {
        await _authenticationService.SendOtp(null,emailData.Email,SystemConstants.AuthenticationOtp);
        return ResponseHelper.SuccessResponse(null, MessageConstants.MailSent);
    }

    [HttpPost("forgot-password")]
    [ValidateModel]
    public async Task<IActionResult> ForgotPassword(LoginEmailDto emailData)
    {
        await _authenticationService.ForgotPassword(emailData.Email);
        return ResponseHelper.SuccessResponse(null, MessageConstants.MailSent);
    }

    [HttpPost("reset-password")]
    [ValidateModel]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto passwordDetails, string token)
    {
        await _authenticationService.ResetPassword(passwordDetails.Password, token);
        return ResponseHelper.SuccessResponse(null, MessageConstants.PasswordReset);
    }
    #endregion Method
}
