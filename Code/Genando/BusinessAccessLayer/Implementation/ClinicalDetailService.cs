using AutoMapper;
using BusinessAccessLayer.Abstraction;
using Common.Constants;
using Common.Enums;
using DataAccessLayer.Abstraction;
using Entities.DataModels;
using Entities.DTOs.Request;
using Entities.DTOs.Response;

namespace BusinessAccessLayer.Implementation
{
    public class ClinicalDetailService : GenericService<ClinicalDetail>, IClinicalDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClinicalProcessService _clinicalProcessService;
        private readonly IClinicalQuestionService _clinicalQuestionService;
        private readonly IProfileService _profileService;
        private readonly IClinicalEnhancementAnsRepository _clinicalEnhancementAnsRepository;
        public ClinicalDetailService(IUnitOfWork unitOfWork, IMapper mapper, IClinicalProcessService clinicalProcessService,
            IClinicalQuestionService clinicalQuestionService, IProfileService profileService, IClinicalEnhancementAnsRepository clinicalEnhancementAnsRepository)
            : base(unitOfWork.ClinicalDetailRepository, unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _clinicalProcessService = clinicalProcessService;
            _clinicalQuestionService = clinicalQuestionService;
            _profileService = profileService;
            _clinicalEnhancementAnsRepository = clinicalEnhancementAnsRepository;
        }

        public async Task<IEnumerable<ClinicalDetailDTO>> GetClinicalPath(long patientId)
        {
            List<ClinicalDetail> clinicalDetails = await _unitOfWork.ClinicalDetailRepository.GetAllAsync(detail => detail.PatientId == patientId);
            if (clinicalDetails.Count == 0) return Enumerable.Empty<ClinicalDetailDTO>();

            IEnumerable<int> questionIds = clinicalDetails.Select(detail => detail.QuestionId).Distinct();
            List<ClinicalQuestion> questions = await _unitOfWork.ClinicalQuestionRepository.GetAllAsync(question => questionIds.Contains(question.Id));
            List<ClinicalDetailDTO> clinicalQuestionDTOs = new();

            foreach (ClinicalDetail clinicalDetail in clinicalDetails)
            {
                ClinicalDetailDTO clinicalQuestionDTO = _mapper.Map<ClinicalDetailDTO>(clinicalDetail);
                ClinicalQuestion? question = questions.FirstOrDefault(q => q.Id == clinicalDetail.QuestionId);
                clinicalQuestionDTO.Question = question?.Question;
                clinicalQuestionDTOs.Add(clinicalQuestionDTO);
            }

            return clinicalQuestionDTOs;
        }


        public async Task UpdateAnswersAsync(long patientId, List<ClinicalAnswerDTO> answerDTOs)
        {
            List<ClinicalDetail> clinicalDetailsToUpdate = await _unitOfWork.ClinicalDetailRepository.GetAllAsync(a => a.PatientId == patientId);
            foreach (ClinicalAnswerDTO answerDTO in answerDTOs)
            {
                ClinicalDetail? clinicalDetail = await _unitOfWork.ClinicalDetailRepository.GetFirstOrDefaultAsync(detail => detail.PatientId == patientId && detail.Id == answerDTO.Id);
                if (clinicalDetail != null)
                {
                    clinicalDetail.Answer = answerDTO.Answer;
                    clinicalDetailsToUpdate.Add(clinicalDetail);
                }
            }
            await UpdateRangeAsync(clinicalDetailsToUpdate);

            List<ClinicalDetail> updatedClinicalDetails = await _unitOfWork.ClinicalDetailRepository.GetAllAsync(a => a.PatientId == patientId);
            ClinicalProcess? clinicalProcess = await _unitOfWork.ClinicalProcessRepository.GetFirstOrDefaultAsync(a => a.PatientId == patientId);
            if (updatedClinicalDetails.Count == SystemConstants.NumberOfStaticQuestions && !updatedClinicalDetails.Any(a => a.Answer == null) && clinicalProcess != null)
            {
                clinicalProcess.Status = SystemConstants.ClinicalPathStatus;
                clinicalProcess.NextStep = SystemConstants.PrescribeTestStatus;
                await _clinicalProcessService.UpdateAsync(clinicalProcess);
            }

            User? user = await _unitOfWork.ProfileRepository.GetFirstOrDefaultAsync(u => u.Id == patientId);
            user.ConsultationStatus = PatientConsultationStatusType.OnGoing;
            await _profileService.UpdateAsync(user);
        }

        public async Task<TimelineDTO> GetStatusByPatientId(long patientId)
        {
            ClinicalProcess? clinicalProcess = await _clinicalProcessService.GetFirstOrDefaultAsync(a => a.PatientId == patientId);

            if (clinicalProcess == null)
            {
                TimelineDTO timelineDTO = new()
                {
                    Status = SystemConstants.DefaultClinicalStatus
                };
                return timelineDTO;
            }

            TimelineDTO timelineDTO1 = _mapper.Map<TimelineDTO>(clinicalProcess);
            return timelineDTO1;
        }

        public async Task AddClinicalQuestions(long patientId)
        {
            IEnumerable<int> models = await _clinicalQuestionService.GetAllQuestions();
            if (!models.Any()) return;

            List<ClinicalDetail> questions = new();
            foreach (int questionId in models)
            {
                questions.Add(new ClinicalDetail
                {
                    QuestionId = questionId,
                    PatientId = patientId
                });
            }
            await AddRangeAsync(questions);
        }

        public async Task<IEnumerable<ClinicalDetailDTO>> GetAnswersAsync(ClinicalAnswerRequestDTO clinicalAnswerRequest, CancellationToken cancellationToken = default)
        {
            if (clinicalAnswerRequest.IsRequestedAnswer)
            {
                List<RequestMoreInfoQuestion> requestMoreInfoQuestions = await _unitOfWork.RequestMoreInfoQuestionRepository.GetAllAsync(question => question.PatientId == clinicalAnswerRequest.PatientId);
                if (requestMoreInfoQuestions.Count == 0) return Enumerable.Empty<ClinicalDetailDTO>();

                List<ClinicalDetailDTO> requestMoreInfoDTOs = requestMoreInfoQuestions.Select(requestQuestion => _mapper.Map<ClinicalDetailDTO>(requestQuestion)).ToList();
                return requestMoreInfoDTOs;
            }
            else
            {
                List<ClinicalEnhancementAnswer> clinicalEnhancementAnswers = await _clinicalEnhancementAnsRepository.GetAllAsync(ans => ans.PatientId == clinicalAnswerRequest.PatientId);
                if (clinicalEnhancementAnswers.Count == 0) return Enumerable.Empty<ClinicalDetailDTO>();

                List<ClinicalDetailDTO> clinicalDetailDTOs = new();
                foreach (ClinicalEnhancementAnswer clinicalEnhancementAnswer in clinicalEnhancementAnswers)
                {
                    ClinicalEnhancement? clinicalEnhancement = await _unitOfWork.ClinicalEnhancementRepository.GetAsync(detail => detail.Id == clinicalEnhancementAnswer.QuestionId, cancellationToken: cancellationToken);
                    ClinicalDetailDTO clinicalQuestionDTO = new()
                    {
                        Id = clinicalEnhancement!.Id,
                        Answer = clinicalEnhancementAnswer.Answer,
                        Question = clinicalEnhancement.Question
                    };
                    clinicalDetailDTOs.Add(clinicalQuestionDTO);
                }
                return clinicalDetailDTOs;
            }
        }
    }
}