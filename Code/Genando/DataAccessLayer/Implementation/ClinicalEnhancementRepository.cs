using DataAccessLayer.Abstraction;
using DataAccessLayer.Data;
using Entities.DataModels;

namespace DataAccessLayer.Implementation
{
    public class ClinicalEnhancementRepository : GenericRepository<ClinicalEnhancement>, IClinicalEnhancementRepository
    {
        private readonly AppDbContext _context;
        public ClinicalEnhancementRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}