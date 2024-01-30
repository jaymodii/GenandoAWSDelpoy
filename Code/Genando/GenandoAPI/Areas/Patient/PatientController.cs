﻿using BusinessAccessLayer.Abstraction;
using Common.Constants;
using Common.Enums;
using Common.Exceptions;
using Common.Utils.Model;
using Entities.DTOs.Request;
using Entities.DTOs.Response;
using GenandoAPI.ExtAuthorization;
using GenandoAPI.Filters;
using GenandoAPI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace GenandoAPI.Areas.Patient
{
    [Route("api/[controller]")]
    [ApiController]
    [PatientPolicy]
    public class PatientController : ControllerBase
    {
        private readonly IClinicalDetailService _clinicalDetailService;
        private readonly IClinicalEnhancementService _clinicalEnhancementService;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _http;
        private readonly IRequestMoreInfoQuestionService _moreInfoQuestionService;
        private readonly IUserService _userService;

        public PatientController(IClinicalDetailService clinicalQuestionService,
            IClinicalEnhancementService clinicalEnhancementService,
            IRequestMoreInfoQuestionService moreInfoQuestionService,
            IUserService userService,
            IConfiguration config,
            IHttpContextAccessor http)
        {
            _clinicalDetailService = clinicalQuestionService;
            _moreInfoQuestionService = moreInfoQuestionService;
            _clinicalEnhancementService = clinicalEnhancementService;
            _userService = userService;
            _config = config;
            _http = http;
        }

        [HttpGet]
        [Route("getClinicalPath")]
        public async Task<IActionResult> GetClinicalPath()
        {
            LoggedUser loggedUser = new AuthHelper(_http.HttpContext, _config).GetLoggedUser();

            IEnumerable<ClinicalDetailDTO> clinicalDetailDTOs = await _clinicalDetailService.GetClinicalPath(loggedUser.UserId);

            return ResponseHelper.SuccessResponse(clinicalDetailDTOs);
        }

        [HttpPut]
        [Route("clinicalAnswers")]
        [ValidateModel]
        public async Task<IActionResult> ClinicalAnswers(List<ClinicalAnswerDTO> answerDTOs)
        {

            LoggedUser loggedUser = new AuthHelper(_http.HttpContext, _config).GetLoggedUser();

            await _clinicalDetailService.UpdateAnswersAsync(loggedUser.UserId, answerDTOs);

            return ResponseHelper.CreateResourceResponse(null, "Answers updated successfully");
        }

        [HttpGet]
        [Route("getStatusById/{id}")]
        public async Task<IActionResult> GetStatusById(long id)
        {
            TimelineDTO timelineDTO = await _clinicalDetailService.GetStatusByPatientId(id);

            return ResponseHelper.SuccessResponse(timelineDTO);
        }

        #region Request more question endpoints

        [HttpGet("request-more-info")]
        public async Task<IActionResult> LoadRequestMoreQuestions(CancellationToken cancellationToken)
        {
            IEnumerable<PatientMoreInfoResponseDto> moreQuestions
                = await _moreInfoQuestionService.LoadPatientMoreQuestions(cancellationToken);
            return ResponseHelper.SuccessResponse(moreQuestions);
        }

        [HttpPost("request-more-info")]
        [ValidateModel]
        public async Task<IActionResult> SavePatientMoreInformation(PatientMoreInfoRequestDto dto,
            CancellationToken cancellationToken)
        {
            await _moreInfoQuestionService.SavePatientMoreQuestion(dto, cancellationToken);
            return ResponseHelper.SuccessResponse(null, dto.Status == PatientQuestionStatusType.DraftByPatient
                                                                ? MessageConstants.QuestionSavedAsDraft
                                                                : MessageConstants.InfoSentToPatient);
        }
        
        [HttpGet("contact-doctor")]
        
        public async Task<IActionResult> GetDoctorDetails()
        {  
            ContactDoctorDto doctor_details = await _userService.GetDoctorDetails();
            return ResponseHelper.SuccessResponse(doctor_details,String.Empty);
        }

        #endregion

        [HttpGet]
        [Route("getClinicalEnhancement")]
        public async Task<IActionResult> GetClinicalEnhancement()
        {
            LoggedUser loggedUser = new AuthHelper(_http.HttpContext, _config).GetLoggedUser();

            IEnumerable<ClinicalPathListRequestDTO> clinicalDetailDTOs = await _clinicalEnhancementService.GetClinicalPath(loggedUser.UserId);

            return ResponseHelper.SuccessResponse(clinicalDetailDTOs);
        }

        [HttpPost]
        [Route("clinicalEnhancementAnswers")]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<IActionResult> ClinicalEnhancementAnswers([FromBody] List<AnswerEnhancementDTO> answerDTOs)
        {
            LoggedUser loggedUser = new AuthHelper(_http.HttpContext, _config).GetLoggedUser();

            await _clinicalEnhancementService.PostClinicalEnhancementAnswers(loggedUser.UserId, answerDTOs);

            return ResponseHelper.CreateResourceResponse(null, MessageConstants.AnswerUpdated);
        }
    }
}