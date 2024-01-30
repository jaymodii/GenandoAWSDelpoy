using DataAccessLayer.Abstraction;
using DataAccessLayer.Data;
using Entities.DataModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Implementation
{
    public class TestDetailRepository : GenericRepository<TestDetail>, ITestDetailRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<TestDetail> _dbSet;
        public TestDetailRepository(AppDbContext context) : base(context)
        {
            _context = context;
            _dbSet = _context.Set<TestDetail>();
        }
    }
}
