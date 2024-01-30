using Entities.DataModels;
using Entities.DTOs.Request;
using Entities.DTOs.Response;

namespace BusinessAccessLayer.Abstraction;
public interface IRequestMoreInfoQuestionService
    : IGenericService<RequestMoreInfoQuestion>
{
    Task<IEnumerable<PatientQuestionsResponseDto>> GetPatientQuestions(long patientId,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<PatientMoreInfoResponseDto>> LoadPatientMoreQuestions(CancellationToken cancellationToken = default);
    Task SavePatientMoreQuestion(PatientMoreInfoRequestDto dto,
        CancellationToken cancellationToken = default);
    Task SetPatientQuestions(long id,
        PatientMoreQuestionRequestDto dto,
        CancellationToken cancellationToken = default);
}
