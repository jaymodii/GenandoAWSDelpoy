using DataAccessLayer.Abstraction;
using DataAccessLayer.Data;
using Entities.DataModels;

namespace DataAccessLayer.Implementation
{
    public class ClinicalProcessRepository : GenericRepository<ClinicalProcess>, IClinicalProcessRepository
    {
        private readonly AppDbContext _context;
        public ClinicalProcessRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}