using DataAccessLayer.Abstraction;
using DataAccessLayer.Data;
using Entities.DataModels;

namespace DataAccessLayer.Implementation
{
    public class FAQRepository : GenericRepository<FAQ>, IFAQRepository
    {
        private readonly AppDbContext _context;
        public FAQRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}