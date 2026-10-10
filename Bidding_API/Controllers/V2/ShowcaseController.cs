using System.Linq;
using System.Threading.Tasks;
using Bidding_API.Helpers;
using Bidding_Common.Helpers;
using Bidding_Core.Services.Interfaces;
using Bidding_Models.Jobseekers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bidding_API.Controllers.V2
{
    /// <summary>UI showcase sample records (mock data held in memory by <see cref="IShowcaseService"/>).</summary>
    [ApiController]
    [Route("api/v2/showcase/items")]
    [Authorize]
    public class ShowcaseController : ControllerBase
    {
        private readonly IShowcaseService _service;

        public ShowcaseController(IShowcaseService service)
        {
            _service = service;
        }

        private void EnsureAdminCaller()
        {
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
            [FromQuery] string category = null,
            [FromQuery] string status = null)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(() =>
            {
                EnsureAdminCaller();
                return Task.FromResult<IActionResult>(Ok(new { Code = "Success", Data = _service.GetPage(pageIndex, pageSize, q, category, status) }));
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> Get(long id)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(() =>
            {
                EnsureAdminCaller();
                return Task.FromResult<IActionResult>(Ok(new { Code = "Success", Data = _service.Get(id) }));
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UpsertShowcaseItemRequestDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(() =>
            {
                EnsureAdminCaller();
                return Task.FromResult<IActionResult>(Ok(new { Code = "Success", Data = _service.Create(body) }));
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpsertShowcaseItemRequestDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(() =>
            {
                EnsureAdminCaller();
                return Task.FromResult<IActionResult>(Ok(new { Code = "Success", Data = _service.Update(id, body) }));
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(() =>
            {
                EnsureAdminCaller();
                _service.Delete(id);
                return Task.FromResult<IActionResult>(Ok(new { Code = "Success", Data = new { ok = true } }));
            }, ApiResponseHelper.CreateErrorResponse);
        }
    }
}
