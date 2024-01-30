using Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.Request
{
    public class LoginCredentialDto:BaseValidationModel<LoginCredentialDto>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; } = false;
    }
    public class LoginOtpDto : BaseValidationModel<LoginOtpDto>
    {
        public string Email { get; set; } = null!;
        public string Otp { get; set; } = null!;
    }
    public class LoginEmailDto : BaseValidationModel<LoginEmailDto>
    {
        public string Email { get; set; } = null!;
    }
    public class ResetPasswordDto : BaseValidationModel<ResetPasswordDto>
    {
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
    public class TokensDto : BaseValidationModel<TokensDto>
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
