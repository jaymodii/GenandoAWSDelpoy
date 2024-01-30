using DataAccessLayer.Abstraction;
using DataAccessLayer.Data;
using Entities.DataModels;

namespace DataAccessLayer.Implementation;

public class UserRepository :
    GenericRepository<User>, IUserRepository
{
    #region Constructors

    public UserRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    #endregion Constructors
}