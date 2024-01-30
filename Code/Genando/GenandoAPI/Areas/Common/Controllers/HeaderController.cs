using BusinessAccessLayer.Abstraction;
using Common.Utils.Model;
using Entities.DTOs.Response;
using GenandoAPI.ExtAuthorization;
using GenandoAPI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace GenandoAPI.Areas.Common.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeaderController : ControllerBase
    {
        private readonly IFAQService _faqService;
        private readonly IUserService _userService;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _http;
        private readonly ITestResultService _testResultService;
        public HeaderController(IFAQService faqService, IConfiguration config, IHttpContextAccessor http, IUserService userService, ITestResultService testResultService)
        {
            _faqService = faqService;
            _userService = userService;
            _testResultService = testResultService;
            _config = config;
            _http  = http;
        }
        #region FAQ

        [Route("Faq")]
        [HttpGet]
        public async Task<IActionResult> getFaq()
        {
            LoggedUser loggedUser = new AuthHelper(_http.HttpContext, _config).GetLoggedUser();

            byte role = Convert.ToByte(loggedUser.Role);

            IEnumerable<FAQResponseDTO> faqResponseDTO = await _faqService.GetFaq(role);

            return ResponseHelper.SuccessResponse(faqResponseDTO);
        }

        [HttpGet("getAvatar")]
        public async Task<IActionResult> GetAvatar()
        {
            LoggedUser loggedUser = new AuthHelper(_http.HttpContext, _config).GetLoggedUser();

            string Avatar = await _userService.GetAvatar(loggedUser.UserId);

            return ResponseHelper.SuccessResponse(Avatar);
        }

        #endregion

        [HttpGet("DownloadFile/{id:long}")]
        public async Task<IActionResult> DownloadFile(long id, CancellationToken cancellationToken)
        {
            DownloadLabResultDTO downloadLabResult = await _testResultService.GetReportAttachmentAsync(id, cancellationToken);

            return File(downloadLabResult.ReportAttachment, downloadLabResult.ContentType, downloadLabResult.ReportAttachmentTitle);
        }
    }
}
