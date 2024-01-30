using BusinessAccessLayer.Abstraction;
using Common.Constants;
using Common.Exceptions;
using Common.Utils.Model;
using Entities.DataModels;
using Entities.DTOs.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace BusinessAccessLayer.Implementation
{
    public class JwtManageService : IJwtManageService
    {
        #region Properties
        public IConfiguration _configuration;
        public IHttpContextAccessor _httpContext;
        #endregion Properties

        #region Constructor
        public JwtManageService(IConfiguration configuration, IHttpContextAccessor httpContext)
        {
            _configuration = configuration;
            _httpContext = httpContext;
        }
        #endregion Constructor

        #region Interface Method
        public TokensDto GenerateToken(User user)
        {
            return GenerateJwtToken(user);
        }

        public TokensDto GenerateRefreshToken(User user)
        {
            return GenerateJwtToken(user);
        }

        public TokensDto GenerateJwtToken(User user)
        {
            //set key and credential
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //add claim
            var claims = new[]
            {
                new Claim(SystemConstants.UserIdClaim,user.Id.ToString()),
                new Claim(ClaimTypes.Role,user.Role.ToString()),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Name,user.FirstName+" "+user.LastName),
                new Claim(SystemConstants.LabIdClaim,user.LabId.ToString())
            };
            //make token
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: credentials);
            return new TokensDto { AccessToken = new JwtSecurityTokenHandler().WriteToken(token), RefreshToken = GenerateRefreshToken() };
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new Byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal GetPrincipalFormExpiredToken(string token)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenValidatorParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = false,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero,
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidatorParameters, out SecurityToken securityToken);

            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException(MessageConstants.INVALID_TOKEN);
            }
            return principal;
        }
        public LoggedUser GetLoggedUser()
        {
            string authToken = _httpContext.HttpContext.Request.Headers.Authorization.FirstOrDefault() ?? throw new UnauthorizedException(MessageConstants.UNAUTHERIZE);
            var jsonToken = authToken.ToString().Replace(SystemConstants.Bearer, string.Empty);

            ClaimsPrincipal? claims = GetPrincipalFormExpiredToken(jsonToken);

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
        #endregion Interface Method
    }
}
