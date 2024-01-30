using Entities.DataModels;
using Entities.DTOs.Request;
using Entities.DTOs.Response;

namespace BusinessAccessLayer.Abstraction
{
    public interface IClinicalEnhancementService : IGenericService<ClinicalEnhancement>
    {
        Task PostClinicalQuestions(long patientId, List<QuestionEnhancementDTO> questionEnhancementDTOs);

        Task PostClinicalEnhancementAnswers(long patientId, List<AnswerEnhancementDTO> answerEnhancementDTOs);

        Task<IEnumerable<ClinicalPathListRequestDTO>> GetClinicalPath(long patientId);

        Task<IEnumerable<ClinicalEnhancementQuestionDTO>> GetClinicalQuestion(long id, CancellationToken cancellationToken = default);

        Task DeleteClinicalQuestion(long questionId, CancellationToken cancellationToken = default);
    }
}
