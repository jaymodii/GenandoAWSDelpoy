using DataAccessLayer.Abstraction;
using DataAccessLayer.Data;
using Entities.DataModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Implementation;

public class AuthenticationRepository : GenericRepository<User>, IAuthenticationRepository
{
    #region Properties
    public new readonly AppDbContext _dbContext;
    #endregion Properties

    #region Constructor
    public AuthenticationRepository(AppDbContext dbContext):base(dbContext)
    {
        _dbContext = dbContext;
    }
    #endregion Constructor

    #region Interface Method
    public async Task<User> GetUserByEmail(string email) => await _dbContext.Users.FirstOrDefaultAsync(user => user.Email == email);

    public async Task<UserRefreshTokens> AddUserRefreshToken(UserRefreshTokens userRefreshTokens)
    {
        await _dbContext.UserRefreshTokens.AddAsync(userRefreshTokens);
        return userRefreshTokens;
    }

    public async Task<UserRefreshTokens> GetUserRefreshTokens(string email,string refreshToken)
    {
        return await _dbContext.UserRefreshTokens.FirstOrDefaultAsync(urf => urf.Email == email && urf.RefreshToken == refreshToken && urf.IsActive == true);
    }

    public async Task DeleteUserRefreshToken(string email, string refreshToken)
    {
        var urf = await _dbContext.UserRefreshTokens.FirstOrDefaultAsync(urf => urf.Email == email && urf.RefreshToken == refreshToken);
        if (urf != null) { _dbContext.UserRefreshTokens.Remove(urf); }
    }
    #endregion Interface Method
}
