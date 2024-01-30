using AutoMapper;
using BusinessAccessLayer.Abstraction;
using Common.Constants;
using Common.Enums;
using Common.Exceptions;
using Common.Hubs;
using Common.Utils.Model;
using DataAccessLayer.Abstraction;
using DataAccessLayer.Helpers;
using Entities.DataModels;
using Entities.DTOs.Request;
using Entities.DTOs.Response;
using Microsoft.AspNetCore.SignalR;
using System.Linq.Expressions;

namespace BusinessAccessLayer.Implementation
{
    public class ClinicalProcessService : GenericService<ClinicalProcess>, IClinicalProcessService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITestDetailService _testDetailService;
        private readonly IHubContext<BroadcastHub> _hubContext;

        public ClinicalProcessService(IUnitOfWork unitOfWork, IMapper mapper, ITestDetailService testDetailService, IHubContext<BroadcastHub> hubContext)
        : base(unitOfWork.ClinicalProcessRepository, unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _testDetailService = testDetailService;
            _hubContext = hubContext;
        }

       
        public async Task<PageListResponseDTO<PatientListForDoctorDto>> GetAllPatientsForDoctorAsync(long doctorId, PatientListRequestDTO patientListRequest, CancellationToken cancellationToken = default)
        {
            PageListRequestEntity<ClinicalProcess> pageListRequestEntity = _mapper.Map<PatientListRequestDTO, PageListRequestEntity<ClinicalProcess>>(patientListRequest);
            pageListRequestEntity.IncludeExpressions = new Expression<Func<ClinicalProcess, object>>[] { x => x.Users, x => x.ClinicalProcessStatuses };
            pageListRequestEntity.OrderByDescending = x => x.CreatedOn;

            List<User> doctorUsers = await _unitOfWork.UserRepository.GetAllAsync(e => e.DoctorId == doctorId && e.Status == 1 && e.Role == (byte)UserRoleType.Patient);

            List<long> userIds = doctorUsers.Select(user => user.Id).ToList();

            Expression<Func<ClinicalProcess, bool>> clinicalProcessFilter = clinicalProcess => userIds.Contains(clinicalProcess.Users.Id);

            PageListResponseDTO<ClinicalProcess> pageListResponse = await _unitOfWork.ClinicalProcessRepository.GetAllAsync(
                pageListRequestEntity,
                clinicalProcessFilter,
                cancellationToken
            );

            List<PatientListForDoctorDto> patientListForDoctorDto = new();

            foreach (ClinicalProcess clinicalProcess in pageListResponse.Records)
            {
                User? user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(x => x.Id == clinicalProcess.PatientId, cancellationToken);

                List<ClinicalProcessTest>? test = await _unitOfWork.ClinicalProcessTestRepository.GetAllDataAsync(x => x.ClinicalProcessId == clinicalProcess.Id, new Expression<Func<ClinicalProcessTest, object>>[] { x => x.TestDetails });

                string tests = string.Join(", ", test.Select(x => x.TestDetails.Abbreviation));

                ClinicalProcessStatus? nextStep = await _unitOfWork.ClinicalProcessStatusRepository.GetFirstOrDefaultAsync(x => x.Id == clinicalProcess.NextStep, cancellationToken);

                ClinicalProcessStatus? status = await _unitOfWork.ClinicalProcessStatusRepository.GetFirstOrDefaultAsync(x => x.Id == clinicalProcess.Status, cancellationToken);

                patientListForDoctorDto.Add(new PatientListForDoctorDto
                {
                    PatientName = $"{user?.FirstName} {user?.LastName}",
                    Gender = user.Gender,
                    TestTitle = tests == "" ? null : tests,
                    NextStep = nextStep?.Title,
                    Status = status?.Title,
                    ExpectedDate = clinicalProcess?.ExpectedDate,
                    PatientId = clinicalProcess.PatientId,
                    ClinicalProcessId = clinicalProcess.Id
                });
            }

            return new PageListResponseDTO<PatientListForDoctorDto>(pageListResponse.PageIndex, pageListResponse.PageSize, pageListResponse.TotalRecords, patientListForDoctorDto);
        }

        public async Task<PatientTestDetailDTO> GetPatientTestDetailAsync(long clinicalProcessId, CancellationToken cancellationToken)
        {
            ClinicalProcess? clinicalProcess = await _unitOfWork.ClinicalProcessRepository.GetAsync(process => process.Id == clinicalProcessId, new Expression<Func<ClinicalProcess, object>>[] { x => x.Users });

            if (clinicalProcess == null)
                throw new ModelValidationException(MessageConstants.RECORD_NOT_FOUND);

            string imgDataURL = "";

            if (clinicalProcess?.Users.Avatar != null)
            {
                string imreBase64Data = Convert.ToBase64String(clinicalProcess.Users.Avatar);
                imgDataURL = string.Format("data:image/png;base64,{0}", imreBase64Data);
            }

            IEnumerable<TestDetail> testDetails = await _unitOfWork.TestDetailRepository.GetAllAsync();

            List<ClinicalProcessTest> clinicalProcessTests = await _unitOfWork.ClinicalProcessTestRepository.GetAllAsync(x => x.ClinicalProcessId == clinicalProcessId);

            PatientTestDetailDTO patientTestDetailDTO = _mapper.Map<PatientTestDetailDTO>(clinicalProcess);

            patientTestDetailDTO.testDetails = testDetails;

            patientTestDetailDTO.Avatar = imgDataURL;

            patientTestDetailDTO.prescribedTestId = clinicalProcessTests.Select(x => x.TestId).ToList();

            return patientTestDetailDTO;
        }

        public async Task PrescribeTestAsync(PrescribeTestRequestDTO prescribeTestDTO, LoggedUser loggedUser, CancellationToken cancellationToken)
        {

            ClinicalProcess clinicalProcess = await _unitOfWork.ClinicalProcessRepository.GetByIdAsync(prescribeTestDTO.ClinicalProcessId, cancellationToken);

            List<TestDetail> testDetails = await _unitOfWork.TestDetailRepository.GetAllAsync(e => prescribeTestDTO.TestIds.Contains(e.Id));

            if (clinicalProcess == null || testDetails == null || (testDetails.Count() != prescribeTestDTO.TestIds.Count())) throw new ModelValidationException(MessageConstants.RECORD_NOT_FOUND);

            List<ClinicalProcessTest> prescribedClinicalProcess = await _unitOfWork.ClinicalProcessTestRepository.GetAllAsync(e => prescribeTestDTO.TestIds.Contains(e.Id) && e.ClinicalProcessId == prescribeTestDTO.ClinicalProcessId);

            bool containsAll = prescribedClinicalProcess.All(x => prescribeTestDTO.TestIds.Contains(x.Id)) && prescribedClinicalProcess.Count() == prescribeTestDTO.TestIds.Count();

            if (containsAll) throw new ModelValidationException(MessageConstants.TEST_ALREADY_PRESCRIBED);


            List<long> prescribedClinicalProcessIds = prescribedClinicalProcess.Select(x => x.TestId).ToList();

            List<long> testIds = prescribeTestDTO.TestIds.Where(x => !prescribedClinicalProcessIds.Contains(x)).ToList();

            List<ClinicalProcessTest> processTests = new();

            foreach (long item in testIds)
            {
                ClinicalProcessTest clinicalProcessTest = new()
                {
                    ClinicalProcessId = prescribeTestDTO.ClinicalProcessId,
                    TestId = item
                };
                processTests.Add(clinicalProcessTest);
            }

            await _unitOfWork.ClinicalProcessTestRepository.AddRangeAsync(processTests);

            byte duration = testDetails.Max(x => x.Duration);

            clinicalProcess.Status = SystemConstants.PrescribeTestStatus;
            clinicalProcess.NextStep = SystemConstants.CollectSampleStatus;
            clinicalProcess.Deadline = DateTime.Now.AddDays(duration * 7);
            clinicalProcess.ExpectedDate = DateTime.Now.AddDays(duration * 7 + 5);

            User? user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(user => user.Id == clinicalProcess.PatientId);

            Notification notification = new Notification()
            {
                SentBy = loggedUser.UserId,
                SendTo = user.Id,
                NotificationMessage = $"Dr. {loggedUser.Name} has prescribed test for you. You'll need to submit a sample."
            };
            await _unitOfWork.NotificationRepository.AddAsync(notification);

            await _hubContext.Clients.All.SendAsync("BroadcastMessage");

            await UpdateAsync(clinicalProcess);
        }

        #region Patient Clinical Profile

        public async Task<PatientTestInfoResponseDto> LoadPatientProfile(long patientId,
        CancellationToken cancellationToken = default)
        {
            ClinicalProcessTest? model = await _unitOfWork.ClinicalProcessTestRepository.GetAsync(x => x.ClinicalProcesses.PatientId == patientId,
                new Expression<Func<ClinicalProcessTest, object>>[] { x => x.ClinicalProcesses, y => y.TestDetails },
                new string[] { "ClinicalProcesses.Users" }) ?? throw new ResourceNotFoundException(MessageConstants.CLINICAL_PROFILE_NOT_FOUND);

            return _mapper.Map<PatientTestInfoResponseDto>(model);
        }

        #endregion

        #region Clinical process status updation

        public async Task UpdateClinicalProcessStatus(long id, LoggedUser loggedUser,
        CancellationToken cancellationToken = default)
        {
            ClinicalProcessTest clinicalProcessTest = await _unitOfWork.ClinicalProcessTestRepository.GetAsync(x => x.ClinicalProcesses.PatientId == id, new Expression<Func<ClinicalProcessTest, object>>[] { x => x.ClinicalProcesses }, new string[] { "ClinicalProcesses.Users" }) ?? throw new ResourceNotFoundException(MessageConstants.CLINICAL_PROFILE_NOT_FOUND);

            ++clinicalProcessTest.ClinicalProcesses.Status;
            ++clinicalProcessTest.ClinicalProcesses.NextStep;

            await UpdateAsync(clinicalProcessTest.ClinicalProcesses, cancellationToken);

            if (clinicalProcessTest.ClinicalProcesses.Status == SystemConstants.CollectSampleStatus)
            {
                Notification notification = new Notification()
                {
                    SentBy = loggedUser.UserId,
                    SendTo = (long)clinicalProcessTest.ClinicalProcesses.Users.Id,
                    NotificationMessage = $"Your sample has been collected by Dr. {loggedUser.Name} for testing."
                };

                await _unitOfWork.NotificationRepository.AddAsync(notification);

                await _hubContext.Clients.All.SendAsync("BroadcastMessage");
            }

            if (clinicalProcessTest.ClinicalProcesses.Status == SystemConstants.ShipSampleStatus)
            {
                Notification notification = new Notification()
                {
                    SentBy = loggedUser.UserId,
                    SendTo = (long)clinicalProcessTest.ClinicalProcesses.Users.LabId,
                    NotificationMessage = $"Dr. {loggedUser.Name} has prescribed a test and shipped the sample that requires testing for TST00{id}."
                };

                await _unitOfWork.NotificationRepository.AddAsync(notification);

                await _hubContext.Clients.All.SendAsync("BroadcastMessage");
            }

            if (clinicalProcessTest.ClinicalProcesses.Status == SystemConstants.RecieveSampleStatus)
            {

                Notification notification = new Notification()
                {
                    SentBy = loggedUser.UserId,
                    SendTo = (long)clinicalProcessTest.ClinicalProcesses.Users.DoctorId,
                    NotificationMessage = $"Lab analyst {loggedUser.Name} has received the sample of {clinicalProcessTest.ClinicalProcesses.Users.FirstName} {clinicalProcessTest.ClinicalProcesses.Users.LastName}. Wait for the lab results."
                };

                await _unitOfWork.NotificationRepository.AddAsync(notification);

                await _hubContext.Clients.All.SendAsync("BroadcastMessage");
            }
            await _unitOfWork.SaveAsync(cancellationToken);
        }

        #endregion

        #region Helper methods

        private static FilterCriteria<ClinicalProcessTest> ClinicalProcessProfileCriteria(long patientId)
        {
            FilterCriteria<ClinicalProcessTest> criteria = new()
            {
                Filter = PatientClinicalFilter(patientId),
                IncludeExpressions = new()
                {
                    IncludeTestDetail
                },
                ThenIncludeExpressions = new string[] { "ClinicalProcesses.Users" }
            };

            return criteria;
        }

        #endregion

        #region Helper filters

        private static Expression<Func<ClinicalProcessTest, bool>> PatientClinicalFilter(long patientId)
            => model => model.ClinicalProcesses.PatientId == patientId;

        private static Expression<Func<ClinicalProcess, object>> IncludeUser
        => model => model.Users;

        private static Expression<Func<ClinicalProcessTest, object>> IncludeTestDetail
            => model => model.TestDetails;

        #endregion
    }
}