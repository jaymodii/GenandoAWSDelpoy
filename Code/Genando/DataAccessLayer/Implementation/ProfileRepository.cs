using DataAccessLayer.Abstraction;
using DataAccessLayer.Data;
using Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Implementation
{
    public class ProfileRepository : GenericRepository<User>, IProfileRepository
    {
        private readonly AppDbContext _context;
        public ProfileRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> IsDuplicateEmail(string email, long? userId, CancellationToken cancellationToken = default)
        => userId is null ? await AnyAsync(EmailFilter(email), cancellationToken) : await AnyAsync(EmailFilter(email, userId), cancellationToken);


        #region helper method
        private static Expression<Func<User, bool>> EmailFilter(string email,
        long? userId = null)
        => userId is null ? user => user.Email == email
                            : user => user.Email == email && user.Id != userId;

        #endregion
    }

}
