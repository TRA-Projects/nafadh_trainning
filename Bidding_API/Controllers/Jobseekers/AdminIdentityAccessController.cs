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
    [ApiController]
    [Route("api/jobseekers/admin/identity")]
    [Authorize]
    public class AdminIdentityAccessController : ControllerBase
    {
        private readonly IIdentityAccessAdminService _identityAccessAdminService;

        public AdminIdentityAccessController(IIdentityAccessAdminService identityAccessAdminService)
        {
            _identityAccessAdminService = identityAccessAdminService;
        }

        private int? TryGetProfileId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "ProfileId");
            return (claim != null && int.TryParse(claim.Value, out var pid) && pid > 0)
                ? pid
                : (int?)null;
        }

        private bool IsAdminCaller()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "VendorType");
            return claim != null && int.TryParse(claim.Value, out var vendorType) && vendorType == 0;
        }

        private void EnsureAdminCaller()
        {
            if (TryGetProfileId() == null)
            {
                throw new SafeExecutionHelper.UnauthorizedException("A valid ProfileId claim is required.");
            }

            if (!IsAdminCaller())
            {
                throw new SafeExecutionHelper.ForbiddenException("This resource is restricted to Admin users.");
            }
        }

        private string CallerUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = 5,
            [FromQuery] string q = null,
            [FromQuery] int? userTypeId = null,
            [FromQuery] int? userGroupId = null,
            [FromQuery] bool? isActive = null)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                var page = await _identityAccessAdminService.GetUsersPageAsync(
                    pageIndex,
                    pageSize,
                    q,
                    userTypeId,
                    userGroupId,
                    isActive,
                    CallerUserId());
                return Ok(new { Code = "Success", Data = page });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateAdminUser([FromBody] CreateAdminUserRequestDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                var row = await _identityAccessAdminService.CreateAdminUserAsync(body, CallerUserId());
                return Ok(new { Code = "Success", Data = row });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateAdminUserRequestDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                var row = await _identityAccessAdminService.UpdateAdminUserAsync(id, body, CallerUserId());
                return Ok(new { Code = "Success", Data = row });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPut("users/{id}/group")]
        public async Task<IActionResult> ChangeUserGroup(string id, [FromBody] ChangeUserGroupRequestDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                if (body == null || body.UserGroupId <= 0)
                {
                    throw new SafeExecutionHelper.BadRequestException("UserGroupId is required.");
                }

                var row = await _identityAccessAdminService.ChangeUserGroupAsync(id, body.UserGroupId, CallerUserId());
                return Ok(new { Code = "Success", Data = row });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("users/{id}/accounts")]
        public async Task<IActionResult> AddAccount(string id, [FromBody] ChangeUserGroupRequestDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                if (body == null || body.UserGroupId <= 0)
                {
                    throw new SafeExecutionHelper.BadRequestException("UserGroupId is required.");
                }

                var row = await _identityAccessAdminService.AddProfileAccountAsync(id, body.UserGroupId, CallerUserId());
                return Ok(new { Code = "Success", Data = row });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> RemoveAccount(string id)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                await _identityAccessAdminService.RemoveProfileAccountAsync(id, CallerUserId());
                return Ok(new { Code = "Success", Data = true });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpGet("groups")]
        public async Task<IActionResult> GetGroups([FromQuery] bool includeInactive = false)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                var catalog = await _identityAccessAdminService.GetGroupsCatalogAsync(includeInactive);
                return Ok(new { Code = "Success", Data = catalog });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("groups")]
        public async Task<IActionResult> CreateGroup([FromBody] UpsertUserGroupRequestDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                var row = await _identityAccessAdminService.CreateUserGroupAsync(body, CallerUserId());
                return Ok(new { Code = "Success", Data = row });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPut("groups/{id:int}")]
        public async Task<IActionResult> UpdateGroup(int id, [FromBody] UpsertUserGroupRequestDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                var row = await _identityAccessAdminService.UpdateUserGroupAsync(id, body, CallerUserId());
                return Ok(new { Code = "Success", Data = row });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpDelete("groups/{id:int}")]
        public async Task<IActionResult> DeactivateGroup(int id)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                await _identityAccessAdminService.DeactivateUserGroupAsync(id, CallerUserId());
                return Ok(new { Code = "Success", Data = true });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpGet("organizations")]
        public async Task<IActionResult> GetOrganizations(
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = 5,
            [FromQuery] string q = null)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                var page = await _identityAccessAdminService.GetOrganizationsPageAsync(pageIndex, pageSize, q);
                return Ok(new { Code = "Success", Data = page });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpGet("pages")]
        public async Task<IActionResult> GetPages()
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                var pages = await _identityAccessAdminService.GetPagesCatalogAsync();
                return Ok(new { Code = "Success", Data = pages });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPut("pages/{pageId:int}/requirements")]
        public async Task<IActionResult> UpdatePageRequirements(int pageId, [FromBody] UpdatePageRequirementsRequestDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                var page = await _identityAccessAdminService.UpdatePageRequirementsAsync(pageId, body, CallerUserId());
                return Ok(new { Code = "Success", Data = page });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles(
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = 5,
            [FromQuery] string q = null,
            [FromQuery] int? userTypeId = null,
            [FromQuery] int? userGroupId = null,
            [FromQuery] bool? templatesOnly = null)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                var page = await _identityAccessAdminService.GetRolesPageAsync(
                    pageIndex,
                    pageSize,
                    q,
                    userTypeId,
                    userGroupId,
                    templatesOnly);
                return Ok(new { Code = "Success", Data = page });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] CreateCustomRoleRequestDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                var row = await _identityAccessAdminService.CreateCustomRoleAsync(body, CallerUserId());
                return Ok(new { Code = "Success", Data = row });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpDelete("roles/{roleId}")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                await _identityAccessAdminService.DeleteCustomRoleAsync(roleId, CallerUserId());
                return Ok(new { Code = "Success", Data = true });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpGet("roles/{roleId}/pages")]
        public async Task<IActionResult> GetRolePages(string roleId)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                var data = await _identityAccessAdminService.GetRolePagesAsync(roleId);
                return Ok(new { Code = "Success", Data = data });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPut("roles/{roleId}/pages")]
        public async Task<IActionResult> UpdateRolePages(string roleId, [FromBody] UpdateRolePagesRequestDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                EnsureAdminCaller();
                var data = await _identityAccessAdminService.UpdateRolePagesAsync(roleId, body, CallerUserId());
                return Ok(new { Code = "Success", Data = data });
            }, ApiResponseHelper.CreateErrorResponse);
        }
    }
}
