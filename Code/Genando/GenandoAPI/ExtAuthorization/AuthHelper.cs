using Common.Constants;
using Common.Exceptions;
using Common.Utils.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GenandoAPI.ExtAuthorization
{
    public class AuthHelper
    {
        #region Properties
        public HttpContext _httpContext;
        public IConfiguration _confi;
        #endregion Properties

        #region Constructor
        public AuthHelper(HttpContext httpContext, IConfiguration configuration)
        {
            _httpContext = httpContext;
            _confi = configuration;
        }
        #endregion Constructor

        #region Method
        internal void AuthorizeRequest()
        {
            string authToken = _httpContext.Request.Headers.Authorization.FirstOrDefault() ?? throw new UnauthorizedException(MessageConstants.UNAUTHERIZE);
            var jsonToken = authToken.ToString().Replace(SystemConstants.Bearer, string.Empty);

            JwtSetting jwtSetting = GetJwtSetting(_confi);

            ClaimsPrincipal? claims = GetClaimsWithValidationToken(jwtSetting, jsonToken) ?? throw new UnauthorizedAccessException();

            // Create the CurrentUserModel object from the claims
            SetLoggedUser(_httpContext, claims);
        }
        #endregion Method

        #region Helper Method
        private static JwtSetting GetJwtSetting(IConfiguration configuration)
        {
            JwtSetting jwtSetting = new();
            configuration.GetSection("Jwt").Bind(jwtSetting);
            return jwtSetting;
        }

        private static void SetLoggedUser(HttpContext httpContext, ClaimsPrincipal claims)
        {
            LoggedUser loggedUser = new()
            {
                UserId = Convert.ToInt64(claims.FindFirstValue(SystemConstants.UserIdClaim) ?? SystemConstants.ZeroString),
                Role = Convert.ToInt32(claims.FindFirstValue(ClaimTypes.Role) ?? SystemConstants.ZeroString),
                Name = claims.FindFirstValue(ClaimTypes.Name).ToString(),
                Email = claims.FindFirstValue(ClaimTypes.Email).ToString(),
                LabId = Convert.ToInt64(claims.FindFirstValue(SystemConstants.LabIdClaim) != "" ?
                    claims.FindFirstValue(SystemConstants.LabIdClaim) : SystemConstants.ZeroString),
            };

            // Set the authenticated user
            var identity = new ClaimsIdentity(claims.Identity);
            var principal = new ClaimsPrincipal(identity);
            httpContext.User = principal;

            // Attach the CurrentUserModel to the HttpContext.User
            httpContext.Items[SystemConstants.LoggedUser] = loggedUser;
        }

        public ClaimsPrincipal? GetClaimsWithValidationToken(JwtSetting jwtSetting, string jsonToken)
        {

            JwtSecurityTokenHandler tokenHandler = new();

            byte[] key = Encoding.ASCII.GetBytes(jwtSetting.Key);

            TokenValidationParameters validationParameters = new()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidIssuer = jwtSetting.Issuer,
                ClockSkew = TimeSpan.Zero
            };

            IsTokenExpire(jsonToken);

            ClaimsPrincipal? claims = tokenHandler.ValidateToken(jsonToken, validationParameters, out var validatedToken);
            return claims;
        }

        public void IsTokenExpire(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);

            if (jsonToken.ValidTo < DateTime.UtcNow) throw new UnauthorizedException(MessageConstants.TOKEN_EXPIRE);
        }

        public LoggedUser GetLoggedUser()
        {
            string authToken = _httpContext.Request.Headers.Authorization.FirstOrDefault() ?? throw new UnauthorizedException(MessageConstants.UNAUTHERIZE);
            var jsonToken = authToken.ToString().Replace(SystemConstants.Bearer, string.Empty);

            JwtSetting jwtSetting = GetJwtSetting(_confi);

            ClaimsPrincipal? claims = GetClaimsWithValidationToken(jwtSetting, jsonToken);

            return new LoggedUser
            {
                UserId = Convert.ToInt64(claims.FindFirstValue(SystemConstants.UserIdClaim) ?? SystemConstants.ZeroString),
                Role = Convert.ToInt32(claims.FindFirstValue(ClaimTypes.Role) ?? SystemConstants.ZeroString),
                Name = claims.FindFirstValue(ClaimTypes.Name).ToString(),
                Email = claims.FindFirstValue(ClaimTypes.Email).ToString(),
                LabId = Convert.ToInt64(claims.FindFirstValue(SystemConstants.LabIdClaim) != "" ?
                    claims.FindFirstValue(SystemConstants.LabIdClaim) : SystemConstants.ZeroString),
            };
        }
        #endregion Helper Method
    }
}
