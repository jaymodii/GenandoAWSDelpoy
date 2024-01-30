using Common.Utils.Model;
using Entities.DataModels;
using Entities.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAccessLayer.Abstraction
{
    public interface IJwtManageService
    {
        TokensDto GenerateToken(User user);
        TokensDto GenerateRefreshToken(User user);
        ClaimsPrincipal GetPrincipalFormExpiredToken(string token);
        LoggedUser GetLoggedUser();
    }
}
