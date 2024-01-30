using Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstraction
{
    public interface IProfileRepository : IGenericRepository<User>
    {
        Task<bool> IsDuplicateEmail(string email, long? userId, CancellationToken cancellationToken = default);
    }
}
