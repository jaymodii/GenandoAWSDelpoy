using DataAccessLayer.Abstraction;
using DataAccessLayer.Data;
using Entities.DataModels;

namespace DataAccessLayer.Implementation;

public class ClinicalQuestionRepository
    : GenericRepository<ClinicalQuestion>, IClinicalQuestionRepository
{
    #region Constructors

    public ClinicalQuestionRepository(AppDbContext dbContext)
        : base(dbContext)
    {
    }

    #endregion Constructors

}