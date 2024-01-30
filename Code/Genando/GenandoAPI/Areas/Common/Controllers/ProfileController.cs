using BusinessAccessLayer.Abstraction;
using Common.Constants;
using Common.Exceptions;
using Common.Utils.Model;
using Entities.DataModels;
using Entities.DTOs.Request;
using GenandoAPI.ExtAuthorization;
using GenandoAPI.Filters;
using GenandoAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GenandoAPI.Areas.Common.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllPolicy]
    public class ProfileController : ControllerBase
    {

        #region Properties

        private readonly IProfileService _profileService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IJwtManageService _jwtManageService;

        #endregion

        #region Constructor
        public ProfileController(IProfileService profileService, IAuthenticationService authenticationService, IJwtManageService jwtManageService)
        {
            _profileService = profileService;
            _authenticationService = authenticationService;
            _jwtManageService = jwtManageService;
        }

        #endregion

        #region Method

        [HttpGet("getProfileDetails/{id}")]
        public async Task<IActionResult> GetProfileDetails(long id)
        {
            UserDetailsInfoDTO userDetailsInfoDTO = await _profileService.GetProfileDetails(id);

            return ResponseHelper.SuccessResponse(userDetailsInfoDTO);
        }


        [HttpPut("updateProfileDetails/{id}")]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<IActionResult> Update(long id, [FromForm] ProfileDetailsDto profileDetailsDto)
        {

            await _profileService.UpdateUserProfile(id, profileDetailsDto);

            return ResponseHelper.SuccessResponse(profileDetailsDto, MessageConstants.ProfileUpdated);
        }

        [HttpPost("sendProfileOtp")]
        public async Task<IActionResult> SendOtpForProfile([FromBody] string email)
        {
            LoggedUser loggedUser = _jwtManageService.GetLoggedUser();

            await _authenticationService.SendOtp(loggedUser.UserId, email, SystemConstants.ProfileUpdateOtp);
            return ResponseHelper.SuccessResponse(null, MessageConstants.MailSent);
        }

        [HttpPost("verifyProfileOtp")]
        public async Task<IActionResult> VerifyOtp([FromBody] string otp)
        {
            LoggedUser loggedUser = _jwtManageService.GetLoggedUser();

            if (otp != null) await _profileService.VerifyProfileOtp(loggedUser.UserId, otp);

            return ResponseHelper.SuccessResponse(null, message: MessageConstants.OtpVerified);
        }

        [HttpGet("getGenders")]
        public async Task<IActionResult> GetGenders()
        {
            IEnumerable<Gender> genders = await _profileService.GetGenders(); 
            return ResponseHelper.SuccessResponse(genders);
        }

        #endregion
    }
}
