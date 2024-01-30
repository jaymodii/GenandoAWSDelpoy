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

namespace BusinessAccessLayer.Implementation
{
    public class ClinicalEnhancementService : GenericService<ClinicalEnhancement>, IClinicalEnhancementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClinicalProcessService _clinicalProcessService;
        private readonly IUserService _userService;
        private readonly IJwtManageService _jwtManageService;
        private readonly IClinicalEnhancementAnsRepository _clinicalEnhancementAnsRepository;
        private readonly IHubContext<BroadcastHub> _hubContext;

        public ClinicalEnhancementService(IUnitOfWork unitOfWork, IMapper mapper, IJwtManageService jwtManageService,
            IClinicalProcessService clinicalProcessService, IUserService userService, IClinicalEnhancementAnsRepository clinicalEnhancementAnsRepository, IHubContext<BroadcastHub> hubContext)
            : base(unitOfWork.ClinicalEnhancementRepository, unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _clinicalProcessService = clinicalProcessService;
            _userService = userService;
            _jwtManageService = jwtManageService;
            _clinicalEnhancementAnsRepository = clinicalEnhancementAnsRepository;
            _hubContext = hubContext;
        }

        public async Task PostClinicalQuestions(long patientId, List<QuestionEnhancementDTO> questionEnhancementDTOs)
        {
            LoggedUser loggedUser = _jwtManageService.GetLoggedUser();

            if (questionEnhancementDTOs == null || !questionEnhancementDTOs.Any())
                throw new InvalidModelStateException(MessageConstants.NoQuestionsProvided);

            if( patientId != 0 && (await _userService.GetFirstOrDefaultAsync(p=>p.Id == patientId))?.ConsultationStatus == PatientConsultationStatusType.OnGoing )
                throw new InvalidModelStateException(MessageConstants.PATIENT_ANSWERED);
            foreach (var questionEnhancementDTO in questionEnhancementDTOs)
            {
                await ProcessQuestion(patientId, loggedUser.UserId, questionEnhancementDTO);
            }
            
            if(patientId != 0)
            {
                Notification notification = new()
                {
                    SentBy = loggedUser.UserId,
                    SendTo = patientId,
                    NotificationMessage = $"Dr. {loggedUser.Name} has added clinical questions for you. Your response is awaited."
                };

                await _unitOfWork.NotificationRepository.AddAsync(notification);

                await _hubContext.Clients.All.SendAsync("BroadcastMessage");

                await _unitOfWork.SaveAsync();
            }

        }

        private async Task ProcessQuestion(long patientId,long doctorId, QuestionEnhancementDTO questionEnhancementDTO)
        {           
            bool isTextArea = questionEnhancementDTO.TypeOfQuestion == QuestionType.TextArea;
            bool isValidOptions = IsValidOptions(questionEnhancementDTO.Options, isTextArea);
            ClinicalEnhancement? clinicalEnhancement = await _unitOfWork.ClinicalEnhancementRepository.GetFirstOrDefaultAsync(a => a.DoctorId == doctorId && a.Id == questionEnhancementDTO.QuestionId);

            if (clinicalEnhancement == null)
            {
                clinicalEnhancement = _mapper.Map<ClinicalEnhancement>(questionEnhancementDTO);
                clinicalEnhancement.DoctorId = doctorId;
                clinicalEnhancement.LevelOfQuestion = patientId == 0 ? QuestionLevel.DoctorLevel : QuestionLevel.PatientLevel;
                clinicalEnhancement.PatientId = patientId == 0 ? null : patientId;
                if (isTextArea) clinicalEnhancement.Options = null;
                await AddAsync(clinicalEnhancement);
            }
            else
            {
                _mapper.Map(questionEnhancementDTO, clinicalEnhancement);
                clinicalEnhancement.DoctorId = doctorId;
                clinicalEnhancement.LevelOfQuestion = patientId == 0 ? QuestionLevel.DoctorLevel : QuestionLevel.PatientLevel;
                clinicalEnhancement.PatientId = patientId == 0 ? null : patientId;
                if (isTextArea) clinicalEnhancement.Options = null;
                await UpdateAsync(clinicalEnhancement);
            }
        }

        private static bool IsValidOptions(string? options, bool isTextArea)
        {
            if (isTextArea) return true;

            string[]? optionsArray = options?.Split(',')
                .Select(option => option.Trim())
                .Where(option => !string.IsNullOrWhiteSpace(option))
                .ToArray();

            if (optionsArray == null || optionsArray.Length < 2) throw new ForbiddenException(MessageConstants.MINIMUM_TWO_OPTIONS_REQUIRED);

            return true;
        }

        public async Task PostClinicalEnhancementAnswers(long patientId, List<AnswerEnhancementDTO> answerEnhancementDTOs)
        {
            IEnumerable<ClinicalEnhancement> mandatoryQuestions = await GetMandatoryQuestions(patientId);
            Dictionary<long, string> answersDictionary = answerEnhancementDTOs.ToDictionary(dto => dto.Id, dto => dto.Answer);

            foreach (var item in answerEnhancementDTOs)
            {
                if (mandatoryQuestions.Any(x => x.Id == item.Id && x.IsQuestionMandatory == true) && (item.Answer == null || item.Answer == "")) 
                    throw new Exception(MessageConstants.MANDATORY_ANSWER);
            }

            ClinicalProcess? clinicalProcess = await _unitOfWork.ClinicalProcessRepository.GetAsync(a => a.PatientId == patientId);
            if (clinicalProcess!.Status >= SystemConstants.PrescribeTestStatus) throw new ModelValidationException(MessageConstants.CANT_GIVE_ANS_TEST_ALREADY_PRESCRIBE);
            clinicalProcess.Status = SystemConstants.ClinicalPathStatus;
            clinicalProcess.NextStep = SystemConstants.PrescribeTestStatus;
            await _clinicalProcessService.UpdateAsync(clinicalProcess);

            foreach (AnswerEnhancementDTO answerDTO in answerEnhancementDTOs)
            {
                ClinicalEnhancementAnswer? clinicalEnhancementAnswers = await _clinicalEnhancementAnsRepository.GetFirstOrDefaultAsync(ans => ans.PatientId == patientId && ans.QuestionId == answerDTO.Id);
                if (clinicalEnhancementAnswers == null)
                {
                    ClinicalEnhancementAnswer? clinicalEnhancementAnswer = new()
                    {
                        PatientId = patientId,
                        QuestionId = answerDTO.Id,
                        Answer = answerDTO.Answer
                    };
                    await _clinicalEnhancementAnsRepository.AddAsync(clinicalEnhancementAnswer);
                }
                else
                {
                    clinicalEnhancementAnswers.Answer = answerDTO.Answer;
                    clinicalEnhancementAnswers.PatientId = patientId;
                    clinicalEnhancementAnswers.QuestionId = answerDTO.Id;
                    await _clinicalEnhancementAnsRepository.UpdateAsync(clinicalEnhancementAnswers);
                }
                await _unitOfWork.SaveAsync();
            }

            User? user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(u => u.Id == patientId);
            user.ConsultationStatus = PatientConsultationStatusType.OnGoing;

            Notification notification = new Notification()
            {
                SentBy = patientId,
                SendTo = (long)user.DoctorId,
                NotificationMessage = $"You have received response from {user.FirstName} {user.LastName} regarding the clinical questions. Please review it and prescribe test."
            };

            await _unitOfWork.NotificationRepository.AddAsync(notification);

            await _hubContext.Clients.All.SendAsync("BroadcastMessage");

            await _userService.UpdateAsync(user);
        }

        private async Task<IEnumerable<ClinicalEnhancement>> GetMandatoryQuestions(long patientId)
        {
            return await GetAllAsync(a => a.PatientId == patientId && a.IsQuestionMandatory);
        }

        public async Task<IEnumerable<ClinicalPathListRequestDTO>> GetClinicalPath(long patientId)
        {
            long? doctorId = await GetDoctorIdByPatientId(patientId);

            //get question that are related to specific patient
            IEnumerable<ClinicalEnhancement> clinicalDetails =
                (await GetAllAsync(detail => detail.LevelOfQuestion == QuestionLevel.HQLevel
                || (detail.DoctorId == doctorId && detail.LevelOfQuestion == QuestionLevel.DoctorLevel)
                || (detail.PatientId == patientId && detail.LevelOfQuestion == QuestionLevel.PatientLevel))).ToList();

            List<ClinicalPathListRequestDTO> clinicalPathListRequestDTOs = new();
            foreach (var clinicalDetail in clinicalDetails)
            {
                ClinicalPathListRequestDTO clinicalPathListRequestDTO = _mapper.Map<ClinicalPathListRequestDTO>(clinicalDetail);
                clinicalPathListRequestDTO.Answer = (await _clinicalEnhancementAnsRepository.GetFirstOrDefaultAsync(ans => ans.QuestionId == clinicalDetail.Id && ans.PatientId == patientId))?.Answer;
                clinicalPathListRequestDTOs.Add(clinicalPathListRequestDTO);
            }
            return clinicalPathListRequestDTOs;
        }

        public async Task<IEnumerable<ClinicalEnhancementQuestionDTO>> GetClinicalQuestion(long id, CancellationToken cancellationToken = default)
        {
            List<ClinicalEnhancement> question = new();
            question = id == 0
                ? (await GetAllAsync(q => q.DoctorId == _jwtManageService.GetLoggedUser().UserId && q.PatientId == null)).ToList()
                : (await GetAllAsync(q => q.DoctorId == _jwtManageService.GetLoggedUser().UserId && q.PatientId == id)).ToList();
            List<ClinicalEnhancementQuestionDTO> clinicalEnhancementquestion = new();
            foreach (var clinicalDetail in question)
            {
                ClinicalEnhancementQuestionDTO clinicalEnhancementQuestionDTO = new()
                {
                    QuestionId = clinicalDetail.Id,
                    IsQuestionMandatory = clinicalDetail.IsQuestionMandatory,
                    Question = clinicalDetail.Question,
                    Options = clinicalDetail.Options,
                    TypeOfQuestion = clinicalDetail.TypeOfQuestion,
                };
                clinicalEnhancementquestion.Add(clinicalEnhancementQuestionDTO);
            }
            return clinicalEnhancementquestion;
        }

        public async Task DeleteClinicalQuestion(long questionId, CancellationToken cancellationToken = default)
        {
            await RemoveAsync((await GetFirstOrDefaultAsync(e => e.Id == questionId && e.DoctorId == _jwtManageService.GetLoggedUser().UserId, cancellationToken))!, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
        }

        public async Task<long?> GetDoctorIdByPatientId(long patientId)
        {
            FilterCriteria<User> criteria = new()
            {
                Filter = e => e.Id == patientId,
                Select = e => new User()
                {
                    DoctorId = e.DoctorId,
                }
            };
            return (await _userService.GetAsync(criteria))?.DoctorId;
        }
    }
}
