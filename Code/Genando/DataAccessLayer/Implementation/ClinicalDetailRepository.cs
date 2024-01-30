using DataAccessLayer.Abstraction;
using DataAccessLayer.Data;
using Entities.DataModels;

namespace DataAccessLayer.Implementation;
public class ClinicalDetailRepository
    : GenericRepository<ClinicalDetail>, IClinicalDetailRepository
{
    #region Properties

    #endregion

    #region Constructors

    public ClinicalDetailRepository(AppDbContext dbContext)
        : base(dbContext)
    {
    }

    #endregion

    #region Methods
    #endregion

}
