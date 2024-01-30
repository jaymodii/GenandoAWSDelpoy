using DataAccessLayer.Abstraction;
using DataAccessLayer.Data;
using Entities.DataModels;

namespace DataAccessLayer.Implementation
{
    public class ClinicalProcessTestRepository : GenericRepository<ClinicalProcessTest>, IClinicalProcessTestRepository
    {
        private readonly AppDbContext _context;
        public ClinicalProcessTestRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
