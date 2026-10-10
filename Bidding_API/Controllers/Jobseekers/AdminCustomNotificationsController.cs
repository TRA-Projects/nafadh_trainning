using System.Collections.Generic;
using System.Linq;
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
    /// <summary>Admin-authored email / SMS / system notifications with scheduling and targeting.</summary>
    [ApiController]
    [Route("api/jobseekers/admin/custom-notifications")]
    [Authorize]
    public class AdminCustomNotificationsController : ControllerBase
    {
        private readonly ICustomNotificationService _service;

        public AdminCustomNotificationsController(ICustomNotificationService service)
        {
            _service = service;
        }

        private void EnsureAdminCaller()
        {
            var profileClaim = User.Claims.FirstOrDefault(c => c.Type == "ProfileId");
            if (profileClaim == null || !int.TryParse(profileClaim.Value, out var pid) || pid <= 0)
            {
                throw new SafeExecutionHelper.UnauthorizedException("A valid ProfileId claim is required.");
            }

            var vendorClaim = User.Claims.FirstOrDefault(c => c.Type == "VendorType");
            if (vendorClaim == null || !int.TryParse(vendorClaim.Value, out var vendorType) || vendorType != 0)
            {
                throw new SafeExecutionHelper.ForbiddenException("This resource is restricted to Admin users.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPage(
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = 5,
            [FromQuery] string q = null,
            [FromQuery] string status = null,
            [FromQuery] string channel = null)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                var page = await _service.GetPageAsync(pageIndex, pageSize, q, status, channel);
                return Ok(new { Code = "Success", Data = page });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> Get(long id)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                return Ok(new { Code = "Success", Data = await _service.GetAsync(id) });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UpsertCustomNotificationRequestDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                var callerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return Ok(new { Code = "Success", Data = await _service.CreateAsync(body, callerUserId) });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpsertCustomNotificationRequestDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                return Ok(new { Code = "Success", Data = await _service.UpdateAsync(id, body) });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("{id:long}/cancel")]
        public async Task<IActionResult> Cancel(long id)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                return Ok(new { Code = "Success", Data = await _service.CancelAsync(id) });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("recipients-preview")]
        public async Task<IActionResult> PreviewRecipients([FromBody] List<CustomNotificationTargetDto> targets)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                return Ok(new { Code = "Success", Data = await _service.PreviewRecipientsAsync(targets) });
            }, ApiResponseHelper.CreateErrorResponse);
        }
    }
}
