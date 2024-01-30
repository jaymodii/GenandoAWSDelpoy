using DataAccessLayer.Abstraction;
using DataAccessLayer.Data;
using Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Implementation
{
    public class GenderRepository : GenericRepository<Gender>, IGenderRepository
    {
        public GenderRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
