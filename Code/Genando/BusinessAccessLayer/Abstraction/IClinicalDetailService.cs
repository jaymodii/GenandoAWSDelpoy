using Entities.DataModels;
using Entities.DTOs.Request;
using Entities.DTOs.Response;

namespace BusinessAccessLayer.Abstraction
{
    public interface IClinicalDetailService : IGenericService<ClinicalDetail>
    {
        Task<IEnumerable<ClinicalDetailDTO>> GetClinicalPath(long patientId);

        Task UpdateAnswersAsync(long patientId, List<ClinicalAnswerDTO> answerDTOs);

        Task<TimelineDTO> GetStatusByPatientId(long patientId);

        Task AddClinicalQuestions(long patientId);

        Task<IEnumerable<ClinicalDetailDTO>> GetAnswersAsync(ClinicalAnswerRequestDTO clinicalAnswerRequest, CancellationToken cancellationToken = default);
    }
}
