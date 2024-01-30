using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace GenandoAPI.ExtAuthorization
{
    public class ExtAuthorizeFilter : IAuthorizationFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;


        public ExtAuthorizeFilter(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (_httpContextAccessor == null || _httpContextAccessor.HttpContext == null)
                throw new UnauthorizedAccessException();

            new AuthHelper(_httpContextAccessor.HttpContext, _configuration).AuthorizeRequest();
        }
    }
}
