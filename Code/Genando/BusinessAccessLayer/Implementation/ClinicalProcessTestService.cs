using AutoMapper;
using BusinessAccessLayer.Abstraction;
using Common.Constants;
using Common.Exceptions;
using DataAccessLayer.Abstraction;
using DataAccessLayer.Helpers;
using Entities.DataModels;
using Entities.DTOs.Request;
using Entities.DTOs.Response;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace BusinessAccessLayer.Implementation
{
    public class ClinicalProcessTestService : GenericService<ClinicalProcessTest>, IClinicalProcessTestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClinicalProcessService _clinicalProcessService;

        public ClinicalProcessTestService(IUnitOfWork unitOfWork, IMapper mapper, IClinicalProcessService clinicalProcessService) : base(unitOfWork.ClinicalProcessTestRepository, unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _clinicalProcessService = clinicalProcessService;
        }

        public async Task<PageListResponseDTO<PatientInfoDTO>> GetAllPatientAsync(long Id, PatientListRequestDTO patientListRequest, CancellationToken cancellationToken = default)
        {
            PageListRequestEntity<ClinicalProcessTest> pageListRequestEntity = _mapper.Map<PatientListRequestDTO, PageListRequestEntity<ClinicalProcessTest>>(patientListRequest);

            pageListRequestEntity.IncludeExpressions = new Expression<Func<ClinicalProcessTest, object>>[] { x => x.ClinicalProcesses, y => y.TestDetails };

            pageListRequestEntity.OrderByDescending = x => x.CreatedOn;
            pageListRequestEntity.ThenIncludeExpressions = new string[] { "ClinicalProcesses.Users" };

            List<User> labUsers = await _unitOfWork.UserRepository.GetAllAsync(e => e.LabId == Id);
            List<long> userIds = labUsers.Select(user => user.Id).ToList();

            Expression<Func<ClinicalProcessTest, bool>> clinicalProcessTestFilter = clinicalProcess => userIds.Contains(clinicalProcess.ClinicalProcesses.Users.Id);

            PageListResponseDTO<ClinicalProcessTest> pageListResponse = await _unitOfWork.ClinicalProcessTestRepository.GetAllAsync(
                pageListRequestEntity,
                clinicalProcessTestFilter,
                cancellationToken
            );

            List<PatientInfoDTO> patientDTOs = new();

            foreach (ClinicalProcessTest clinicalProcessTest in pageListResponse.Records)
            {
                TestResult? testResult = await _unitOfWork.TestResultRepository.GetFirstOrDefaultAsync(x => x.ClinicalProcessTestId == clinicalProcessTest.Id, cancellationToken);

                if (clinicalProcessTest.ClinicalProcesses.Status >= SystemConstants.ShipSampleStatus)
                {
                    patientDTOs.Add(new PatientInfoDTO
                    {
                        ClinicalProcessTestId = clinicalProcessTest.Id,
                        PatientId = clinicalProcessTest.ClinicalProcesses.PatientId,
                        Test = clinicalProcessTest.TestDetails.Title,
                        Notes = testResult?.LabNotes ?? "NA",
                        Deadline = clinicalProcessTest.ClinicalProcesses.Deadline ?? DateTimeOffset.UtcNow.AddDays(10),
                        isResultUploaded = testResult != null,
                        isSampleRecieve = clinicalProcessTest.ClinicalProcesses.NextStep != SystemConstants.RecieveSampleStatus,
                        isResultsPublished = clinicalProcessTest.ClinicalProcesses.NextStep == SystemConstants.CompleteStatus,
                    });
                }
            }

            patientDTOs.GroupBy(x => x.PatientId);

            return new PageListResponseDTO<PatientInfoDTO>(pageListResponse.PageIndex, pageListResponse.PageSize, pageListResponse.TotalRecords, patientDTOs);
        }

        public async Task<PatientDataDTO> GetUserDetails(long cid)
        {
            bool isExist = await _unitOfWork.ClinicalProcessTestRepository.IsEntityExist(result => result.Id == cid);

            if (!isExist) throw new ResourceNotFoundException(MessageConstants.RECORD_NOT_FOUND);

            ClinicalProcessTest? clinicalProcessTest = await _unitOfWork.ClinicalProcessTestRepository.GetAsync(a => a.Id == cid, new Expression<Func<ClinicalProcessTest, object>>[] { x => x.ClinicalProcesses, y => y.TestDetails}, new string[] { "ClinicalProcesses.Users" });

            if (clinicalProcessTest?.ClinicalProcesses.Status < 6) throw new ModelValidationException(MessageConstants.SAMPLE_NOT_RECEIVED);

            if (clinicalProcessTest.ClinicalProcesses.Users == null)
                throw new ResourceNotFoundException(MessageConstants.RECORD_NOT_FOUND);

            TestResult? testResult = await _unitOfWork.TestResultRepository.GetAsync(x => x.ClinicalProcessTestId == cid);

            PatientDataDTO userDTO = _mapper.Map<PatientDataDTO>(clinicalProcessTest.ClinicalProcesses.Users);

            TestResultListingRequestDTO testResultListings = _mapper.Map<TestResultListingRequestDTO>(testResult);

            string testTitle = clinicalProcessTest.TestDetails.Title;

            userDTO.Gender = userDTO.GenderId == 1 ? SystemConstants.Male : SystemConstants.Female;
            userDTO.Age = DateTimeOffset.Now.Year - userDTO.DOB.Year;
            userDTO.TestResults = testResultListings;
            userDTO.TestTitle = testTitle;
            userDTO.ExternalLink = clinicalProcessTest.ClinicalProcesses.ExternalLink;

            return userDTO;
        }

        public async Task<PatientDetailsDto> GetPatientDetailsAsync(long id, CancellationToken cancellationToken)
        {
            ClinicalProcess model = await _clinicalProcessService.GetAsync(ClinicalProcessProfileCriteria(id), cancellationToken)
                ?? throw new ResourceNotFoundException(MessageConstants.CLINICAL_PROFILE_NOT_FOUND);

            List<ClinicalProcessTest> clinicalProcessTests = await _unitOfWork.ClinicalProcessTestRepository.GetAllDataAsync(x => x.ClinicalProcesses.Users.Id == id, new Expression<Func<ClinicalProcessTest, object>>[] { y => y.TestDetails });

            string testTitles = "";
            double testPrice = 0;
            DateTimeOffset ed = new();
            if (clinicalProcessTests.Count > 0)
            {
                testPrice = clinicalProcessTests.Sum(e=>e.TestDetails.Price);
                var a = clinicalProcessTests.Max(e => e.TestDetails.Duration) * 7 + 5;
                ed = DateTimeOffset.Now.AddDays(a);
                var testTitleArray = clinicalProcessTests.Select(e => e.TestDetails.Abbreviation);
                testTitles = string.Join(", ", testTitleArray);
            }

            return new PatientDetailsDto
            {
                Name = model.Users.FirstName + " " + model.Users.LastName,
                Gender = model.Users.Gender,
                DOB = model.Users.DOB,
                Email = model.Users.Email,
                TestName = testTitles,
                TestPrice = testPrice,
                EstimatedDate = ed,
                PrescribedTestIds = clinicalProcessTests.Select(x => x.Id).ToList(),
            };
        }

        private static FilterCriteria<ClinicalProcess> ClinicalProcessProfileCriteria(long patientId)
        {
            FilterCriteria<ClinicalProcess> criteria = new()
            {
                Filter = PatientClinicalFilter(patientId),
                IncludeExpressions = new()
                {
                    IncludeUser
                }
            };

            return criteria;
        }

        private static Expression<Func<ClinicalProcess, bool>> PatientClinicalFilter(long patientId)
            => model => model.PatientId == patientId;

        private static Expression<Func<ClinicalProcess, object>> IncludeUser
            => model => model.Users;
    }
}
