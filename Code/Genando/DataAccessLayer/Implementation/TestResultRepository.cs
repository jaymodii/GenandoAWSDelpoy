using DataAccessLayer.Abstraction;
using DataAccessLayer.Data;
using Entities.DataModels;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Implementation
{
    public class TestResultRepository : GenericRepository<TestResult>, ITestResultRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<TestResult> _dbSet;
        public TestResultRepository(AppDbContext context) : base(context)
        {
            _context = context;
            _dbSet = _context.Set<TestResult>();
        }
    }
}