using BusinessAccessLayer.Abstraction;
using Common.Constants;
using Common.Hubs;
using Common.Utils.Model;
using Entities.DTOs.Request;
using GenandoAPI.ExtAuthorization;
using GenandoAPI.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GenandoAPI.Areas.Common.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ISwipeActionSettingService _swipeActionSettingService;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _http;

        public NotificationController(INotificationService notificationService, ISwipeActionSettingService swipeActionSettingService, IConfiguration config,
            IHttpContextAccessor http)
        {
            _notificationService = notificationService;
            _swipeActionSettingService = swipeActionSettingService;
            _config = config;
            _http = http;
        }


        [HttpGet("getNotificationsCount")]
        public async Task<IActionResult> GetNotificationsCount()
        {
            LoggedUser loggedUser = await GetLoggedUserAsync();

            int notificationCount = await _notificationService.GetNotificationCountAsync(loggedUser.UserId);

            return ResponseHelper.SuccessResponse(notificationCount);
        }

        [HttpGet("getNotifications")]
        public async Task<IActionResult> GetNotifications()
        {
            LoggedUser loggedUser = await GetLoggedUserAsync();

            IEnumerable<NotificationResultDTO> notificationResultDTO = await _notificationService.GetNotificationResultsAsync(loggedUser.UserId);

            return ResponseHelper.SuccessResponse(notificationResultDTO);
        }

        [HttpGet("readNotifications")]
        public async Task<IActionResult> ReadNotifications(long? notificationId)
        {
            LoggedUser loggedUser = await GetLoggedUserAsync();

            await _notificationService.ReadNotificationsAsync(loggedUser.UserId, notificationId);

            return ResponseHelper.SuccessResponse(null);
        }

        [HttpDelete("deleteNotifications")]
        public async Task<IActionResult> DeleteNotifications(long? notificationId)
        {
            LoggedUser loggedUser = await GetLoggedUserAsync();

            await _notificationService.DeleteNotificationsAsync(loggedUser.UserId, notificationId);

            return ResponseHelper.SuccessResponse(null);
        }

        [HttpGet("getSwipeSetting")]
        public async Task<IActionResult> GetSwipeSetting()
        {
            LoggedUser loggedUser = await GetLoggedUserAsync();

            SwipeActionSettingDTO swipeActionSettingDTO = await _swipeActionSettingService.GetSwipeSettingAsync(loggedUser.UserId);

            return ResponseHelper.SuccessResponse(swipeActionSettingDTO);
        }

        [HttpPost("saveSwipeSetting")]
        public async Task<IActionResult> SaveSwipeSetting(SwipeActionSettingDTO swipeActionSettingDTO)
        {
            LoggedUser loggedUser = await GetLoggedUserAsync();

            await _swipeActionSettingService.SaveSwipeSettingAsync(loggedUser.UserId, swipeActionSettingDTO);

            return ResponseHelper.SuccessResponse(null);
        }
        #region helper Methods

        private async Task<LoggedUser> GetLoggedUserAsync()
        {
            return new AuthHelper(_http.HttpContext, _config).GetLoggedUser();
        }

        #endregion
    }
}
