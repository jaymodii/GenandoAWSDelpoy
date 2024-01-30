using Entities.DataModels;

namespace DataAccessLayer.Abstraction;
public interface IRequestMoreInfoQuestionRepository
    : IGenericRepository<RequestMoreInfoQuestion>
{
    Task DeletePatientQuestions(long id, List<long> deletedQuestionsId, CancellationToken cancellationToken);
}
