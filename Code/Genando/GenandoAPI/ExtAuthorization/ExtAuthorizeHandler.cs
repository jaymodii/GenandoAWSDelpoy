using Common.Constants;
using Common.Enums;
using Common.Exceptions;
using Common.Utils.Model;
using Entities.DTOs.Request;
using Microsoft.AspNetCore.Authorization;

namespace GenandoAPI.ExtAuthorization
{
    public class ExtAuthorizeHandler : AuthorizationHandler<ExtAuthorizeRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public ExtAuthorizeHandler(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExtAuthorizeRequirement requirement)
        {
            HttpContext? httpContext = _httpContextAccessor.HttpContext;
            
            if (httpContext == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            new AuthHelper(httpContext, _configuration).AuthorizeRequest();

            if (CheckUserType(httpContext, requirement))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }

        private static bool CheckUserType(HttpContext context, ExtAuthorizeRequirement requirement)
        {
            LoggedUser? loggedUser = context.Items[SystemConstants.LoggedUser] as LoggedUser;

            if (loggedUser == null) return false;

            // Handle the Policy requirement
            if (requirement.PolicyName == SystemConstants.DoctorPolicy)
            {
                if (loggedUser.Role == (int)UserRoleType.Doctor) return true;
            }
            else if (requirement.PolicyName == SystemConstants.PatientPolicy)
            {
                if (loggedUser.Role == (int)UserRoleType.Patient) return true;
            }
            else if (requirement.PolicyName == SystemConstants.LabUserPolicy)
            {
                if (loggedUser.Role == (int)UserRoleType.Lab) return true;
            }
            else if (requirement.PolicyName == SystemConstants.AllUserPolicy)
            {
                if (loggedUser.Role == (int)UserRoleType.Lab || loggedUser.Role == (int)UserRoleType.Doctor || loggedUser.Role == (int)UserRoleType.Patient) return true;
            }

            throw new UnauthorizedException(MessageConstants.UNAUTHERIZE);
        }
    }
}
