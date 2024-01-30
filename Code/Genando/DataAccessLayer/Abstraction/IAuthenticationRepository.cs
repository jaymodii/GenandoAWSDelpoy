using Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstraction;

public interface IAuthenticationRepository:IGenericRepository<User>
{
    Task<User> GetUserByEmail(string email);
    Task<UserRefreshTokens> AddUserRefreshToken(UserRefreshTokens userRefreshTokens);
    Task<UserRefreshTokens> GetUserRefreshTokens(string email, string refreshToken);
    Task DeleteUserRefreshToken(string email, string refreshToken);
}
