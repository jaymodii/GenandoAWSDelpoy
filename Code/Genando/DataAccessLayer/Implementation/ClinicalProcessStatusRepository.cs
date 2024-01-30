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
    public class ClinicalProcessStatusRepository : GenericRepository<ClinicalProcessStatus>,IClinicalProcessStatusRepository
    {
        private readonly AppDbContext _context;
        public ClinicalProcessStatusRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
