using Common.Utils.Model;
using Entities.DataModels;
using Entities.DTOs.Request;
using Entities.DTOs.Response;

namespace BusinessAccessLayer.Abstraction
{
    public interface IClinicalProcessService : IGenericService<ClinicalProcess>
    {
        Task<PageListResponseDTO<PatientListForDoctorDto>> GetAllPatientsForDoctorAsync(long Id, PatientListRequestDTO patientListRequest, CancellationToken cancellationToken = default);
        
        Task<PatientTestDetailDTO> GetPatientTestDetailAsync(long clinicalProcessId, CancellationToken cancellationToken);

        Task PrescribeTestAsync(PrescribeTestRequestDTO prescribeTestDTO, LoggedUser loggedUser, CancellationToken cancellationToken);

        Task<PatientTestInfoResponseDto> LoadPatientProfile(long patientId, CancellationToken cancellationToken = default);

        Task UpdateClinicalProcessStatus(long id, LoggedUser loggedUser, CancellationToken cancellationToken = default);
    }

}
