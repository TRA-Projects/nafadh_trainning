using System.Security.Claims;
using System.Threading.Tasks;
using Bidding_API.Helpers;
using Bidding_Common.Helpers;
using Bidding_Core.Services.Interfaces;
using Bidding_Models.Jobseekers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bidding_API.Controllers.Jobseekers
{
    /// <summary>Signed-in user's in-app (system) notification inbox. User id always comes from the token.</summary>
    [ApiController]
    [Route("api/jobseekers/notifications")]
    [Authorize]
    public class MyNotificationsController : ControllerBase
    {
        private readonly ICustomNotificationService _service;

        public MyNotificationsController(ICustomNotificationService service)
        {
            _service = service;
        }

        private string CallerUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet]
        public async Task<IActionResult> GetPage(
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = 5,
            [FromQuery] bool unreadOnly = false)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var page = await _service.GetMyNotificationsAsync(CallerUserId(), pageIndex, pageSize, unreadOnly);
                return Ok(new { Code = "Success", Data = page });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var count = await _service.GetMyUnreadCountAsync(CallerUserId());
                return Ok(new { Code = "Success", Data = new UserNotificationUnreadCountDto { Count = count } });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("{id:long}/read")]
        public async Task<IActionResult> MarkRead(long id)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                return Ok(new { Code = "Success", Data = await _service.MarkReadAsync(CallerUserId(), id) });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllRead()
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var updated = await _service.MarkAllReadAsync(CallerUserId());
                return Ok(new { Code = "Success", Data = new UserNotificationUnreadCountDto { Count = 0 }, Updated = updated });
            }, ApiResponseHelper.CreateErrorResponse);
        }
    }
}
