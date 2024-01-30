using DataAccessLayer.Abstraction;
using DataAccessLayer.Data;
using Entities.DataModels;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Implementation;
public class RequestMoreInfoQuestionRepository
    : GenericRepository<RequestMoreInfoQuestion>, IRequestMoreInfoQuestionRepository
{
    #region Properties and fields

    private readonly AppDbContext _dbContext;

    #endregion

    #region Constructor

    public RequestMoreInfoQuestionRepository(AppDbContext dbContext)
        : base(dbContext)
    {
        _dbContext = dbContext;
    }

    #endregion

    #region Interface methods

    public async Task DeletePatientQuestions(long id,
        List<long> deletedQuestionsId,
        CancellationToken cancellationToken)
    {
        string query = $@"UPDATE requestMoreInfoQuestion SET status = 0 
                        WHERE patient_id = {id}
                        AND id IN ( " + string.Join(",", deletedQuestionsId) + " );";

        await _dbContext.Database.ExecuteSqlRawAsync(query, cancellationToken);
    }

    #endregion
}
