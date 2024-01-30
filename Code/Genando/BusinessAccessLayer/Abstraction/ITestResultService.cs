using Common.Utils.Model;
using Entities.DataModels;
using Entities.DTOs.Request;
using Entities.DTOs.Response;

namespace BusinessAccessLayer.Abstraction
{
    public interface ITestResultService : IGenericService<TestResult>
    {
        Task AddLabResultAsync(TestResultDTO testResultDto, LoggedUser loggedUser, CancellationToken cancellationToken = default);

        Task<PatientLabResultDTO> GetLabResultAsync(long clinicalProcessId, CancellationToken cancellationToken = default);

        Task<DownloadLabResultDTO> GetReportAttachmentAsync(long testResultId, CancellationToken cancellationToken);

        Task AddDoctorResultAsync(List<DoctorResultDTO> doctorResultDTO, LoggedUser loggedUser, CancellationToken cancellationToken);
    }
}
