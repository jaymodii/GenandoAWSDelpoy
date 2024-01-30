using BusinessAccessLayer.Abstraction;
using Entities.DTOs.Request;
using GenandoAPI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace GenandoAPI.Areas.Common.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllPolicy]
    public class TestExplanationController : ControllerBase
    {
        private readonly ITestDetailService _testDetailService;
        public TestExplanationController(ITestDetailService testDetailService)
        {
            _testDetailService = testDetailService;
        }

        [HttpGet]
        [Route("testInfo")]
        public async Task<IActionResult> GetTestDetails()
        {
            IEnumerable<TestDetailInfoDTO> testDetailInfoDTO = await _testDetailService.GetTestDetails();

            return ResponseHelper.SuccessResponse(testDetailInfoDTO);
        }
    }
}
