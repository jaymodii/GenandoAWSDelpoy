using Entities.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAccessLayer.Abstraction;

public interface IAuthenticationService
{
    Task<string> Login(LoginCredentialDto userCredential);
    Task<TokensDto> VerifyOtp(long? id, LoginOtpDto otpData,bool rememberMe);
    Task<TokensDto> RefreshToken(TokensDto token);
    Task SendOtp(long? id, string email, string typeOfOtp);
    Task ForgotPassword(string email);
    Task ResetPassword(string password,string token);
}
