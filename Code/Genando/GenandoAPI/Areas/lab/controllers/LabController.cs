using BusinessAccessLayer.Abstraction;
using Common.Constants;
using Common.Exceptions;
using Common.Utils.Model;
using Entities.DTOs.Request;
using Entities.DTOs.Response;
using GenandoAPI.Filters;
using GenandoAPI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace GenandoAPI.Areas.Lab
{
    [Route("api/[controller]")]
    [ApiController]
    [LabUserPolicy]
    public class LabController : ControllerBase
    {
        #region Properties

        private readonly IClinicalProcessService _clinicalProcessService;
        private readonly ITestResultService _testResultService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IClinicalProcessTestService _clinicalProcessTestService;
        private readonly IJwtManageService _jwtManageService;

        #endregion Properties

        #region Constructor

        public LabController(IClinicalProcessService clinicalProcessService, ITestResultService testResultService, IHttpContextAccessor contextAccessor, IConfiguration configuration, IClinicalProcessTestService clinicalProcessTestService, IJwtManageService jwtManageService)
        {
            _clinicalProcessService = clinicalProcessService;
            _testResultService = testResultService;
            _contextAccessor = contextAccessor;
            _configuration = configuration;
            _clinicalProcessTestService = clinicalProcessTestService;
            _jwtManageService = jwtManageService;
        }

        #endregion Constructor

        #region Action Methods

        [HttpPost("patientList/{Id}")]
        public async Task<IActionResult> GetList(long Id, [FromBody] PatientListRequestDTO patientListRequest, CancellationToken cancellationToken)
        {
            patientListRequest ??= new PatientListRequestDTO();

            if (!ModelState.IsValid) throw new ModelValidationException(ModelState);

            PageListResponseDTO<PatientInfoDTO> labDashboardPage = await _clinicalProcessTestService.GetAllPatientAsync(Id, patientListRequest, cancellationToken);

            return ResponseHelper.SuccessResponse(labDashboardPage, string.Empty);
        }

        [HttpGet]
        [Route("getUserById/{id}")]
        public async Task<IActionResult> GetUserById(long id)
        {
            PatientDataDTO patientDTO = await _clinicalProcessTestService.GetUserDetails(id);
            return ResponseHelper.SuccessResponse(patientDTO, string.Empty);
        }

        [HttpPost]
        [Route("postLabResult")]
        [ValidateModel]
        public async Task<IActionResult> CreateTestResult([FromForm] TestResultDTO testResultDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) throw new ModelValidationException(ModelState);

            LoggedUser loggedUser = _jwtManageService.GetLoggedUser();
            await _testResultService.AddLabResultAsync(testResultDto, loggedUser, cancellationToken);

            return ResponseHelper.SuccessResponse(null,MessageConstants.ResultUpload);
        }

        [HttpPut("clinical-process/receive-sample/{id:long}")]
        public async Task<IActionResult> ReceiveSample(long id, CancellationToken cancellationToken)
        {
            LoggedUser loggedUser = _jwtManageService.GetLoggedUser();
            await _clinicalProcessService.UpdateClinicalProcessStatus(id, loggedUser, cancellationToken);
            return ResponseHelper.SuccessResponse(null, MessageConstants.ReceiveSample);
        }
        #endregion Action Methods

    }
}