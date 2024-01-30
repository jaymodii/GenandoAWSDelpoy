using AutoMapper;
using BusinessAccessLayer.Abstraction;
using Common.Constants;
using Common.Enums;
using Common.Exceptions;
using Common.Hubs;
using Common.Utils;
using Common.Utils.Model;
using DataAccessLayer.Abstraction;
using Entities.DataModels;
using Entities.DTOs.Request;
using Entities.DTOs.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Linq.Expressions;

namespace BusinessAccessLayer.Implementation
{
    public class TestResultService : GenericService<TestResult>, ITestResultService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProfileService _profileService;
        private readonly IMailService _mailService;
        private readonly IClinicalProcessService _clinicalProcessService;
        private readonly IHubContext<BroadcastHub> _hubContext;

        public TestResultService(IUnitOfWork unitOfWork, IMapper mapper, IMailService mailService, IHttpContextAccessor httpContextAccessor, IProfileService profileService, IClinicalProcessService clinicalProcessService, IHubContext<BroadcastHub> hubContext) : base(unitOfWork.TestResultRepository, unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _profileService = profileService;
            _mailService = mailService;
            _clinicalProcessService = clinicalProcessService;
            _hubContext = hubContext;
        }

        public async Task AddLabResultAsync(TestResultDTO testResultDto, LoggedUser loggedUser, CancellationToken cancellationToken = default)
        {
            ClinicalProcessTest? clinicalProcessTest = await _unitOfWork.ClinicalProcessTestRepository.GetAsync(x => x.Id == testResultDto.ClinicalProcessTestId);

            if (clinicalProcessTest == null) throw new ResourceNotFoundException(MessageConstants.RECORD_NOT_FOUND);

            TestResult testResult = _mapper.Map<TestResult>(testResultDto);

            using (MemoryStream? memoryStream = new MemoryStream())
            using (BinaryReader? binaryReader = new BinaryReader(testResultDto.ReportAttachment.OpenReadStream()))
            {
                testResult.ReportAttachment = binaryReader.ReadBytes((int)testResultDto.ReportAttachment.Length);
            }

            bool testResultExist = await _unitOfWork.TestResultRepository.AnyAsync(x => x.ClinicalProcessTestId == testResultDto.ClinicalProcessTestId);

            if (!testResultExist)
            {
                await AddAsync(testResult);
            }
            else
            {
                testResult.Id = testResultDto.TestResultId;
                await UpdateAsync(testResult);
            }

            List<ClinicalProcessTest> prescribedTest = await _unitOfWork.ClinicalProcessTestRepository.GetAllDataAsync(x => x.ClinicalProcessId == clinicalProcessTest.ClinicalProcessId);

            List<TestResult> labResultTest = await _unitOfWork.TestResultRepository.GetAllDataAsync(x => x.ClinicalProcessTests.ClinicalProcessId == clinicalProcessTest.ClinicalProcessId);


            if (prescribedTest.Count() == labResultTest.Count())
            {
                ClinicalProcess clinicalProcess = await _unitOfWork.ClinicalProcessRepository.GetByIdAsync(clinicalProcessTest.ClinicalProcessId, cancellationToken);

                clinicalProcess.ExternalLink = testResultDto.ExternalLink;
                clinicalProcess.Status = SystemConstants.SendLabResultStatus;
                clinicalProcess.NextStep = SystemConstants.PublishReportStatus;
                _unitOfWork.ClinicalProcessRepository.Update(clinicalProcess);

                User user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(user => user.Id == clinicalProcess.PatientId);

                Notification notification = new Notification()
                {
                    SentBy = loggedUser.UserId,
                    SendTo = (long)user.DoctorId,
                    NotificationMessage = $"Lab analyst {loggedUser.Name} has uploaded lab results for {user.FirstName} {user.LastName}."
                };

                await _unitOfWork.NotificationRepository.AddAsync(notification);

                await _hubContext.Clients.All.SendAsync("BroadcastMessage");
            }

            await _unitOfWork.SaveAsync();
        }

        public async Task<PatientLabResultDTO> GetLabResultAsync(long clinicalProcessId, CancellationToken cancellationToken = default)
        {

            List<TestResult>? testResults = await _unitOfWork.TestResultRepository.GetAllDataAsync(result => result.ClinicalProcessTests.ClinicalProcessId == clinicalProcessId, new Expression<Func<TestResult, object>>[] { x => x.ClinicalProcessTests }, new string[] { "ClinicalProcessTests.ClinicalProcesses", "ClinicalProcessTests.ClinicalProcesses.Users", "ClinicalProcessTests.TestDetails" });

            if (testResults == null) throw new ModelValidationException(MessageConstants.RECORD_NOT_FOUND);

            PatientLabResultDTO patientLabResult = _mapper.Map<PatientLabResultDTO>(testResults[0]);

            List<TestResultListingRequestDTO> testResultListings = _mapper.Map<List<TestResultListingRequestDTO>>(testResults);

            patientLabResult.TestResults = testResultListings;

            return patientLabResult;
        }


        public async Task<DownloadLabResultDTO> GetReportAttachmentAsync(long testResultId, CancellationToken cancellationToken)
        {
            TestResult? testResult = await _unitOfWork.TestResultRepository.GetFirstOrDefaultAsync(result => result.ClinicalProcessTestId == testResultId, cancellationToken);

            if (testResult == null) throw new ResourceNotFoundException(MessageConstants.RECORD_NOT_FOUND);

            DownloadLabResultDTO downloadLabResultDTO = _mapper.Map<DownloadLabResultDTO>(testResult);

            return downloadLabResultDTO;
        }

        public async Task AddDoctorResultAsync(List<DoctorResultDTO> doctorResultDTO,LoggedUser loggedUser, CancellationToken cancellationToken)
        {
            List<TestResult>? testResult = await _unitOfWork.TestResultRepository.GetAllDataAsync(result => doctorResultDTO.Select(test => test.ClinicalProcessTestId).Contains(result.ClinicalProcessTestId), new Expression<Func<TestResult, object>>[] { x => x.ClinicalProcessTests });

            if (testResult == null) throw new ResourceNotFoundException(MessageConstants.RECORD_NOT_FOUND);

            string doctorNotes = "";

            foreach(TestResult result in testResult)
            {
                result.DoctorNotes = doctorResultDTO.Where(x => x.ClinicalProcessTestId == result.ClinicalProcessTestId).Select(x => x.DoctorNotes).FirstOrDefault();

                doctorNotes += result.DoctorNotes;
            }
            
            long clinicalProcessId = testResult[0].ClinicalProcessTests.ClinicalProcessId;

            List<TestResult> labResults = await _unitOfWork.TestResultRepository.GetAllDataAsync(result => result.ClinicalProcessTests.ClinicalProcessId == clinicalProcessId);

            ClinicalProcess? clinicalProcess = await _unitOfWork.ClinicalProcessRepository.GetAsync(result => result.Id == clinicalProcessId, new Expression<Func<ClinicalProcess, object>>[] { x => x.Users }, new string[] { "Users.DoctorUsers","Users.LabUsers" });

            string patientName = $"{clinicalProcess!.Users.FirstName} {clinicalProcess.Users.LastName}";

            if (labResults.Count() == testResult.Count())
            {
                clinicalProcess.Status = SystemConstants.CompleteStatus;
                clinicalProcess.NextStep = SystemConstants.CompleteStatus;
                await _unitOfWork.ClinicalProcessRepository.UpdateAsync(clinicalProcess, cancellationToken);

                long PatientId = clinicalProcess.PatientId;
                User? user = await _unitOfWork.ProfileRepository.GetFirstOrDefaultAsync(u => u.Id == PatientId);
                user.ConsultationStatus = PatientConsultationStatusType.Done;
                await _profileService.UpdateAsync(user);
            }

            FileConversionDTO? fileConversionDTO = new FileConversionDTO
            {
                ByteFiles = labResults.Select(x => x.ReportAttachment).ToList(),
                FileNames = labResults.Select(x => $"{patientName}_Test_Report").ToList()
            };

            List<IFormFile> testReportFiles = await _mailService.ConvertByteListToFormFiles(fileConversionDTO);

            MailDto mailDto = new()
            {
                ToEmail = clinicalProcess.Users.Email,
                Subject = MailConstants.TestReportMailSubject,
                Body = MailBodyUtil.SendTestReportEmail(patientName, $"{clinicalProcess.Users.DoctorUsers.FirstName} {clinicalProcess.Users.DoctorUsers.LastName}", "", doctorNotes),
            };

            await SendNotificationForPublishResult(loggedUser, clinicalProcess);

            await UpdateRangeAsync(testResult);

            mailDto.Attachments!.AddRange(testReportFiles);

            await _mailService.SendMailAsync(mailDto, cancellationToken);

        }

        private async Task SendNotificationForPublishResult(LoggedUser loggedUser, ClinicalProcess clinicalProcess)
        {
            Notification patientNotification = new()
            {
                SentBy = loggedUser.UserId,
                SendTo = clinicalProcess.PatientId,
                NotificationMessage = $"Your report has been published by Dr. {loggedUser.Name}. Please Check your email for further details."
            };

            Notification labNotification = new()
            {
                SentBy = loggedUser.UserId,
                SendTo = (long)clinicalProcess.Users.LabId,
                NotificationMessage = $"The report has been published by Dr. {loggedUser.Name} for TST00{clinicalProcess.PatientId}."
            };

            await _unitOfWork.NotificationRepository.AddAsync(patientNotification);

            await _unitOfWork.NotificationRepository.AddAsync(labNotification);

            await _hubContext.Clients.All.SendAsync("BroadcastMessage");
        }

        private static Expression<Func<ClinicalProcess, object>> IncludeUser
           => model => model.Users;

        private static Expression<Func<ClinicalProcess, ClinicalProcess>> Select
            => model => new()
            {
                Id=model.Id,
                Users= new User()
                {
                    FirstName = model.Users.FirstName,
                    LastName = model.Users.LastName,
                    Email = model.Users.Email,
                    DoctorUsers= model.Users.DoctorUsers,
                },
            };
    }
}