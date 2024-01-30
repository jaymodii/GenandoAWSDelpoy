using BusinessAccessLayer.Abstraction;
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
using static Common.Constants.SystemConstants;
namespace GenandoAPI.Areas.Doctor.Controllers;

[Route("api/[controller]")]
[ApiController]
[DoctorPolicy]
public class DoctorsController : ControllerBase
{
    #region Properties

    private readonly IUserService _userService;
    private readonly IRequestMoreInfoQuestionService _questionService;
    private readonly IClinicalProcessService _clinicalProcessService;
    private readonly ITestResultService _testResultService;
    private readonly IClinicalEnhancementService _clinicalEnhancementService;
    private readonly IClinicalDetailService _clinicalDetailService;
    private readonly IClinicalProcessTestService _clinicalProcessTestService;
    private readonly IJwtManageService _jwtManageService;

    #endregion Properties

    #region Constructor

    public DoctorsController(IClinicalEnhancementService clinicalEnhancementService, IUserService userService, IClinicalProcessService clinicalProcessService, IRequestMoreInfoQuestionService questionService, ITestResultService testResultService,IClinicalDetailService clinicalDetailService, IClinicalProcessTestService clinicalProcessTestService, IJwtManageService jwtManageService)
    {
        _userService = userService;
        _clinicalProcessService = clinicalProcessService;
        _testResultService = testResultService;
        _clinicalEnhancementService = clinicalEnhancementService;
        _questionService = questionService;
        _clinicalDetailService = clinicalDetailService;
        _clinicalProcessTestService = clinicalProcessTestService;
        _jwtManageService = jwtManageService;
    }

    #endregion Constructor

    #region Action Methods

    [HttpPost("register-user")]
    [ValidateModel]
    public async Task<IActionResult> RegisterUser([FromBody] UserDetailsFormRequestDto request,
        CancellationToken cancellationToken)
    {
        LoggedUser loggedUser = _jwtManageService.GetLoggedUser();

        long userId = await _userService.RegisterAsync(request, loggedUser, cancellationToken);

        return ResponseHelper.CreateResourceResponse(userId, MessageConstants.AccountCreated);
    }

    [HttpPost("patients/search")]
    [ValidateModel] 
    public async Task<IActionResult> Search([FromBody] PatientPageRequestDto dto,
        CancellationToken cancellationToken)
    {
        dto.PageRequest ??= new PageRequestDto();

        LoggedUser loggedUser = _jwtManageService.GetLoggedUser();

        PageInformationDto<UserListingResponseDto> page =
            await _userService.SearchAsync(dto, loggedUser.UserId, cancellationToken);

        return ResponseHelper.SuccessResponse(page, string.Empty);
    }

    [HttpGet("user/{id:long}")]
    public async Task<IActionResult> GetUserDetails(long id,
        CancellationToken cancellationToken)
    {
        UserDetailsFormResponseDto dto
            = await _userService.LoadUserProfile(id, cancellationToken);

        return ResponseHelper.SuccessResponse(dto);
    }

    [HttpGet("lab")]
    public async Task<IActionResult> GetLabDetails(CancellationToken cancellationToken)
    {
        LoggedUser loggedUser = _jwtManageService.GetLoggedUser();

        UserListingResponseDto? dto = await _userService.LoadDoctorLabUser(loggedUser.UserId, cancellationToken);
        return ResponseHelper.SuccessResponse(dto);
    }

    [HttpPut("update-user/{id:long}")]
    [ValidateModel]
    public async Task<IActionResult> EditUserDetails([FromRoute] long id,
        [FromBody] UserDetailsFormRequestDto dto,
        CancellationToken cancellationToken)
    {
        LoggedUser loggedUser = _jwtManageService.GetLoggedUser();

        await _userService.EditUserProfile(id, dto,loggedUser, cancellationToken);

        return ResponseHelper.SuccessResponse(dto,
           message: (dto.IsPatient ? PatientString : LabString)
           + " " +
           MessageConstants.ProfileUpdated);
    }

    [HttpDelete("delete-user/{id:long}")]
    public async Task<IActionResult> DeleteUser(long id,
        CancellationToken cancellationToken)
    {
        await _userService.DeleteUserAsync(id, cancellationToken);

        return ResponseHelper.SuccessResponse(null, message: MessageConstants.ProfileDeleted);
    }

    [HttpGet("patients/request-more-info/{id:long}")]
    public async Task<IActionResult> RequestMoreInfo(long id,
        CancellationToken cancellationToken)
    {
        IEnumerable<PatientQuestionsResponseDto> patientQuestions =
            await _questionService.GetPatientQuestions(id, cancellationToken);

        return ResponseHelper.SuccessResponse(patientQuestions, string.Empty);
    }

    [HttpPost("patients/request-more-info/{id:long}")]
    [ValidateModel]
    public async Task<IActionResult> RequestMoreInfo(long id,
        PatientMoreQuestionRequestDto dto,
        CancellationToken cancellationToken)
    {
        await _questionService.SetPatientQuestions(id, dto, cancellationToken);

        return ResponseHelper.SuccessResponse(null, CreateMessageForRequestInfo(dto));
    }

    [HttpGet("clinical-process/request-clinical-profile/{id:long}")]
    public async Task<IActionResult> RequestPatientClinicalProfile(long id,
        CancellationToken cancellationToken)
    {
        PatientTestInfoResponseDto dto = await _clinicalProcessService.LoadPatientProfile(id, cancellationToken);
        return ResponseHelper.SuccessResponse(dto);
    }

    [HttpPost("patientListForDoctor/{id:long}")]
    [ValidateModel]
    public async Task<IActionResult> GetPatientsListForDoctor(long id, [FromBody] PatientListRequestDTO patientListRequest, CancellationToken cancellationToken)
    {
        patientListRequest ??= new PatientListRequestDTO();

        PageListResponseDTO<PatientListForDoctorDto> doctorDashboardPage = await _clinicalProcessService.GetAllPatientsForDoctorAsync(id, patientListRequest, cancellationToken);

        return ResponseHelper.SuccessResponse(doctorDashboardPage, string.Empty);
    }

    [HttpGet("getLabResult/{id:long}")]
    public async Task<IActionResult> GetLabResult(long id, CancellationToken cancellationToken)
    {
        PatientLabResultDTO patientLabResult = await _testResultService.GetLabResultAsync(id, cancellationToken);

        return ResponseHelper.SuccessResponse(patientLabResult);
    }

    [HttpGet("DownloadFile/{id:long}")]
    public async Task<IActionResult> DownloadFile(long id, CancellationToken cancellationToken)
    {
        DownloadLabResultDTO downloadLabResult = await _testResultService.GetReportAttachmentAsync(id, cancellationToken);

        return File(downloadLabResult.ReportAttachment, downloadLabResult.ContentType, downloadLabResult.ReportAttachmentTitle);
    }

    [HttpPut("SendResult")]
    [ValidateModel]
    public async Task<IActionResult> SendResult([FromBody] List<DoctorResultDTO> doctorResultDTO, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            LoggedUser loggedUser = _jwtManageService.GetLoggedUser();
            await _testResultService.AddDoctorResultAsync(doctorResultDTO, loggedUser, cancellationToken);
        }

        return ResponseHelper.SuccessResponse(null, message: MessageConstants.DoctorResult);
    }

    [HttpGet("PatientDetails/{id:long}")]
    public async Task<IActionResult> PatientDetails(long id,CancellationToken cancellationToken)
    {
        return ResponseHelper.SuccessResponse(await _clinicalProcessTestService.GetPatientDetailsAsync(id,cancellationToken), message: "");
    }

    [HttpPost]
    [Route("AddClinicalQuestions/{patientId:long}")]
    [ValidateModel]
    public async Task<IActionResult> PostQuestions(long patientId, [FromBody] List<QuestionEnhancementDTO> questionEnhancementDTO)
    {
        await _clinicalEnhancementService.PostClinicalQuestions(patientId, questionEnhancementDTO);

        return ResponseHelper.SuccessResponse(null, message: MessageConstants.PostQuestion);
    }

    [HttpGet("GetPatientTestDetails/{id:long}")]
    public async Task<IActionResult> GetPatientTestDeatils(long id, CancellationToken cancellationToken)
    {
        PatientTestDetailDTO patientTestDetailDTO = await _clinicalProcessService.GetPatientTestDetailAsync(id, cancellationToken);

        return ResponseHelper.SuccessResponse(patientTestDetailDTO, message: MessageConstants.DoctorResult);
    }

    [HttpPut("PrescribeTest")]
    [ValidateModel]
    public async Task<IActionResult> PrescribeTest([FromBody] PrescribeTestRequestDTO prescribeTestDTO, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            LoggedUser loggedUser = _jwtManageService.GetLoggedUser();
            await _clinicalProcessService.PrescribeTestAsync(prescribeTestDTO, loggedUser, cancellationToken);
        }
        else
            throw new Exception(MessageConstants.INVALID_ATTEMPT);

        return ResponseHelper.SuccessResponse(null, message: MessageConstants.PrescribeTest);
    }

    [HttpPut("clinical-process/collect-sample/{id:long}")]
    public async Task<IActionResult> CollectSample(long id,
        CancellationToken cancellationToken)
    {
        LoggedUser loggedUser = _jwtManageService.GetLoggedUser();
        await _clinicalProcessService.UpdateClinicalProcessStatus(id, loggedUser, cancellationToken);
        return ResponseHelper.SuccessResponse(null, MessageConstants.CollectSample);
    }

    [HttpPut("clinical-process/ship-sample/{id:long}")]
    public async Task<IActionResult> ShipSample(long id,
        CancellationToken cancellationToken)
    {
        LoggedUser loggedUser = _jwtManageService.GetLoggedUser();
        await _clinicalProcessService.UpdateClinicalProcessStatus(id, loggedUser, cancellationToken);
        return ResponseHelper.SuccessResponse(null, MessageConstants.ShipSample);
    }

    [HttpPost("getAnswers")]
    [ValidateModel]
    public async Task<IActionResult> GetAnswers(ClinicalAnswerRequestDTO clinicalAnswerRequest, CancellationToken cancellationToken = default)
    {
        IEnumerable<ClinicalDetailDTO> clinicalDetailDTO = await _clinicalDetailService.GetAnswersAsync(clinicalAnswerRequest, cancellationToken);

        return ResponseHelper.SuccessResponse(clinicalDetailDTO);
    }
    [HttpGet("getClinicalQuestionForDoctor/{id:long}")]
    public async Task<IActionResult> GetGenericQuestion(long id, CancellationToken cancellationToken = default)
    {
        IEnumerable<ClinicalEnhancementQuestionDTO> genericQuestion = await _clinicalEnhancementService.GetClinicalQuestion(id,cancellationToken);

        return ResponseHelper.SuccessResponse(genericQuestion);
    }

    [HttpDelete("deleteClinicalQuestion/{id:long}")]
    public async Task<IActionResult> DeleteClinicalQuestion(long id, CancellationToken cancellationToken = default)
    {
        await _clinicalEnhancementService.DeleteClinicalQuestion(id,cancellationToken);
        return ResponseHelper.SuccessResponse(null,MessageConstants.DeleteQuestion);
    }

    #endregion Action Methods

    #region Helper methods

    private static string CreateMessageForRequestInfo(PatientMoreQuestionRequestDto dto)
        => dto.Status == PatientQuestionStatusType.DraftByDoctor
            ? MessageConstants.QuestionSavedAsDraft
            : MessageConstants.QuestionSendToPatient;

    #endregion
}