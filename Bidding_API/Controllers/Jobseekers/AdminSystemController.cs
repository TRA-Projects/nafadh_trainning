using System;
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
    [Route("api/jobseekers/admin")]
    [AllowAnonymous]
    public class AdminSystemController : ControllerBase
    {
        private readonly IAdminSystemService _adminSystemService;

        public AdminSystemController(IAdminSystemService adminSystemService)
        {
            _adminSystemService = adminSystemService;
        }

        [HttpGet("system-variables")]
        public async Task<IActionResult> GetSystemVariables(
            [FromQuery] int? pageIndex,
            [FromQuery] int? pageSize,
            [FromQuery] string q = "")
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var data = await _adminSystemService.GetSystemVariablesPageAsync(pageIndex, pageSize, q);
                return Ok(new { Data = data });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPut("system-variables/{variableKey}")]
        public async Task<IActionResult> UpdateSystemVariable(string variableKey, [FromBody] UpdateSystemVariableRequestDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var updated = await _adminSystemService.UpdateSystemVariableAsync(variableKey, body?.Value);
                if (updated == null)
                {
                    return NotFound(new { Code = "NotFound", Message = "System variable not found" });
                }

                return Ok(new { Data = updated });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpGet("system-config/values/{code}/search")]
        public async Task<IActionResult> SearchConfigValues(string code, [FromQuery] string q = "")
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var rows = await _adminSystemService.SearchConfigValuesAsync(code, q);
                return Ok(new { Data = rows });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("system-config/values/{code}/ensure")]
        public async Task<IActionResult> EnsureConfigValue(string code, [FromBody] EnsureSystemConfigValueRequestDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var row = await _adminSystemService.EnsureConfigValueAsync(code, body);
                return Ok(new { Data = row });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpGet("system-config/values/{code}")]
        public async Task<IActionResult> GetConfigValues(
            string code,
            [FromQuery] int? pageIndex,
            [FromQuery] int? pageSize,
            [FromQuery] string parentReasonId = null,
            [FromQuery] bool includeInactive = false)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var data = await _adminSystemService.GetConfigValuesPageAsync(
                    code,
                    pageIndex,
                    pageSize,
                    parentReasonId,
                    includeInactive);
                return Ok(new { Data = data });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("system-config/values/{code}")]
        public async Task<IActionResult> CreateConfigValue(string code, [FromBody] CreateSystemConfigValueRequestDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                try
                {
                    var created = await _adminSystemService.CreateConfigValueAsync(code, body);
                    return Ok(new { Data = created });
                }
                catch (System.InvalidOperationException ex)
                {
                    return BadRequest(new { Code = "BadRequest", Message = ex.Message });
                }
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPut("system-config/values/{code}/{id}")]
        public async Task<IActionResult> UpdateConfigValue(string code, string id, [FromBody] UpdateSystemConfigValueRequestDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var updated = await _adminSystemService.UpdateConfigValueAsync(code, id, body);
                if (updated == null)
                {
                    return NotFound(new { Code = "NotFound", Message = "Config value not found" });
                }

                return Ok(new { Data = updated });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpGet("skill-categories")]
        public async Task<IActionResult> GetSkillCategories(
            [FromQuery] int? pageIndex,
            [FromQuery] int? pageSize,
            [FromQuery] string q = "")
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var data = await _adminSystemService.GetSkillCategoriesPageAsync(pageIndex, pageSize, q);
                return Ok(new { Data = data });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("skill-categories")]
        public async Task<IActionResult> CreateSkillCategory([FromBody] SkillCategoryDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var created = await _adminSystemService.CreateSkillCategoryAsync(body);
                return Ok(new { Data = created });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPut("skill-categories/{id:int}")]
        public async Task<IActionResult> UpdateSkillCategory(int id, [FromBody] SkillCategoryDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var updated = await _adminSystemService.UpdateSkillCategoryAsync(id, body);
                if (updated == null)
                {
                    return NotFound(new { Code = "NotFound", Message = "Skill category not found" });
                }

                return Ok(new { Data = updated });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpGet("skills")]
        public async Task<IActionResult> GetSkills(
            [FromQuery] int? pageIndex,
            [FromQuery] int? pageSize,
            [FromQuery] string q = "",
            [FromQuery] int? categoryId = null,
            [FromQuery] int? kind = null)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var data = await _adminSystemService.GetSkillsPageAsync(pageIndex, pageSize, q, categoryId, kind);
                return Ok(new { Data = data });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPost("skills")]
        public async Task<IActionResult> CreateSkill([FromBody] SkillDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var created = await _adminSystemService.CreateSkillAsync(body);
                return Ok(new { Data = created });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPut("skills/{id:int}")]
        public async Task<IActionResult> UpdateSkill(int id, [FromBody] SkillDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var updated = await _adminSystemService.UpdateSkillAsync(id, body);
                if (updated == null)
                {
                    return NotFound(new { Code = "NotFound", Message = "Skill not found" });
                }

                return Ok(new { Data = updated });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpGet("notification-templates")]
        public async Task<IActionResult> GetNotificationTemplates(
            [FromQuery] int? pageIndex,
            [FromQuery] int? pageSize,
            [FromQuery] string q = "")
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var data = await _adminSystemService.GetNotificationTemplatesPageAsync(pageIndex, pageSize, q);
                return Ok(new { Data = data });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpGet("notification-templates/{actionCode}")]
        public async Task<IActionResult> GetNotificationTemplate(string actionCode)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var row = await _adminSystemService.GetNotificationTemplateAsync(actionCode);
                if (row == null)
                {
                    return NotFound(new { Code = "NotFound", Message = "Notification template not found" });
                }

                return Ok(new { Data = row });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpGet("notification-templates/{actionCode}/placeholders")]
        public async Task<IActionResult> GetNotificationTemplatePlaceholders(string actionCode)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var data = await _adminSystemService.GetNotificationTemplatePlaceholdersAsync(actionCode);
                return Ok(new { Data = data });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpPut("notification-templates/{actionCode}")]
        public async Task<IActionResult> UpdateNotificationTemplate(
            string actionCode,
            [FromBody] UpdateNotificationTemplateRequestDto body)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                try
                {
                    var updated = await _adminSystemService.UpdateNotificationTemplateAsync(actionCode, body);
                    if (updated == null)
                    {
                        return NotFound(new { Code = "NotFound", Message = "Notification template not found" });
                    }

                    return Ok(new { Data = updated });
                }
                catch (System.InvalidOperationException ex)
                {
                    return BadRequest(new { Code = "BadRequest", Message = ex.Message });
                }
            }, ApiResponseHelper.CreateErrorResponse);
        }

        [HttpGet("notification-templates/{actionCode}/preview")]
        public async Task<IActionResult> PreviewNotificationTemplate(string actionCode, [FromQuery] string locale = "en")
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var preview = await _adminSystemService.PreviewNotificationTemplateAsync(actionCode, locale);
                if (preview == null)
                {
                    return NotFound(new { Code = "NotFound", Message = "Notification template not found" });
                }

                return Ok(new { Data = preview });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        /// <summary>Paged field-level audit trail.</summary>
        [HttpGet("audit-trail")]
        public async Task<IActionResult> GetAuditTrail(
            [FromQuery] int? pageIndex,
            [FromQuery] int? pageSize,
            [FromQuery] string module = "",
            [FromQuery] string entityType = "",
            [FromQuery] string entityReference = "",
            [FromQuery] string field = "",
            [FromQuery] string action = "",
            [FromQuery] string source = "",
            [FromQuery] string updatedByUserId = "",
            [FromQuery] DateTime? updatedFrom = null,
            [FromQuery] DateTime? updatedTo = null,
            [FromQuery] string value = "")
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var data = await _adminSystemService.GetAuditTrailPageAsync(
                    pageIndex,
                    pageSize,
                    module,
                    entityType,
                    entityReference,
                    field,
                    action,
                    source,
                    updatedByUserId,
                    updatedFrom,
                    updatedTo,
                    value);
                return Ok(new { Data = data });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        /// <summary>
        /// Periods when a specific field value was in effect for an entity
        /// (ValidFrom = when NewValue became this value; ValidTo = next change).
        /// </summary>
        [HttpGet("audit-trail/value-history")]
        public async Task<IActionResult> GetAuditValueHistory(
            [FromQuery] int? pageIndex,
            [FromQuery] int? pageSize,
            [FromQuery] string module = "",
            [FromQuery] string entityType = "",
            [FromQuery] string entityReference = "",
            [FromQuery] string field = "",
            [FromQuery] string value = "",
            [FromQuery] DateTime? rangeFrom = null,
            [FromQuery] DateTime? rangeTo = null)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var data = await _adminSystemService.GetAuditValueHistoryPageAsync(
                    pageIndex,
                    pageSize,
                    module,
                    entityType,
                    entityReference,
                    field,
                    value,
                    rangeFrom,
                    rangeTo);
                return Ok(new { Data = data });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        /// <summary>Paged outbound email/SMS transaction logs.</summary>
        [HttpGet("notification-transactions")]
        public async Task<IActionResult> GetNotificationTransactions(
            [FromQuery] int? pageIndex,
            [FromQuery] int? pageSize,
            [FromQuery] string channelType = "",
            [FromQuery] string receiver = "",
            [FromQuery] string actionCode = "",
            [FromQuery] bool? isSuccess = null,
            [FromQuery] DateTime? createdFrom = null,
            [FromQuery] DateTime? createdTo = null)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var data = await _adminSystemService.GetNotificationTransactionLogsPageAsync(
                    pageIndex,
                    pageSize,
                    channelType,
                    receiver,
                    actionCode,
                    isSuccess,
                    createdFrom,
                    createdTo);
                return Ok(new { Data = data });
            }, ApiResponseHelper.CreateErrorResponse);
        }

        /// <summary>Re-send the exact payload from a prior notification transaction log.</summary>
        [HttpPost("notification-transactions/{id:long}/resend")]
        public async Task<IActionResult> ResendNotificationTransaction(long id)
        {
            return await SafeExecutionHelper.ExecuteSafelyAsync(async () =>
            {
                var row = await _adminSystemService.ResendNotificationTransactionAsync(id);
                if (row == null)
                {
                    return NotFound(new { Code = "NotFound", Message = "Notification transaction not found" });
                }

                return Ok(new { Data = row });
            }, ApiResponseHelper.CreateErrorResponse);
        }

    }
}
