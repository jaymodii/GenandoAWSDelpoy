using Entities.DataModels;
using Entities.DTOs.Request;
using Entities.DTOs.Response;

namespace BusinessAccessLayer.Abstraction
{
    public interface IClinicalProcessTestService : IGenericService<ClinicalProcessTest>
    {
        Task<PageListResponseDTO<PatientInfoDTO>> GetAllPatientAsync(long Id, PatientListRequestDTO patientListRequest, CancellationToken cancellationToken = default);

        Task<PatientDataDTO> GetUserDetails(long cid);

        Task<PatientDetailsDto> GetPatientDetailsAsync(long id, CancellationToken cancellationToken);
    }
}
