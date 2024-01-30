using Entities.DataModels;

namespace BusinessAccessLayer.Abstraction;
public interface IClinicalQuestionService : IGenericService<ClinicalQuestion>
{
    Task<IEnumerable<int>> GetAllQuestions();
}
