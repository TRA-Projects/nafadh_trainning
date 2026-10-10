using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bidding_Models.Jobseekers;

namespace Bidding_Core.Services.Interfaces
{
    public interface IAdminSystemService
    {
        Task<AdminSystemPagedListDto<SystemVariableDto>> GetSystemVariablesPageAsync(int? pageIndex, int? pageSize, string q);
        Task<SystemVariableDto> UpdateSystemVariableAsync(string variableKey, object value);

        Task<AdminSystemPagedListDto<SystemConfigValueDto>> GetConfigValuesPageAsync(
            string code,
            int? pageIndex,
            int? pageSize,
            string parentReasonId,
            bool includeInactive = false);
        Task<List<SystemConfigValueDto>> SearchConfigValuesAsync(string code, string q);
        Task<SystemConfigValueDto> EnsureConfigValueAsync(string code, EnsureSystemConfigValueRequestDto draft);
        Task<SystemConfigValueDto> CreateConfigValueAsync(string code, CreateSystemConfigValueRequestDto body);
        Task<SystemConfigValueDto> UpdateConfigValueAsync(string code, string id, UpdateSystemConfigValueRequestDto body);

        Task<AdminSystemPagedListDto<SkillCategoryDto>> GetSkillCategoriesPageAsync(int? pageIndex, int? pageSize, string q);
        Task<SkillCategoryDto> CreateSkillCategoryAsync(SkillCategoryDto body);
        Task<SkillCategoryDto> UpdateSkillCategoryAsync(int id, SkillCategoryDto body);

        Task<AdminSystemPagedListDto<SkillDto>> GetSkillsPageAsync(int? pageIndex, int? pageSize, string q, int? categoryId, int? kind);
        Task<SkillDto> CreateSkillAsync(SkillDto body);
        Task<SkillDto> UpdateSkillAsync(int id, SkillDto body);

        Task<string> GetSystemVariableValueAsync(string variableKey);

        Task<AdminSystemPagedListDto<NotificationTemplateListItemDto>> GetNotificationTemplatesPageAsync(int? pageIndex, int? pageSize, string q);
        Task<NotificationTemplateDto> GetNotificationTemplateAsync(string actionCode);
        Task<NotificationTemplatePlaceholdersDto> GetNotificationTemplatePlaceholdersAsync(string actionCode);
        Task<NotificationTemplateDto> UpdateNotificationTemplateAsync(string actionCode, UpdateNotificationTemplateRequestDto body);
        Task<RenderedNotificationDto> PreviewNotificationTemplateAsync(string actionCode, string locale);

        Task<AdminSystemPagedListDto<SystemAuditTrailRowDto>> GetAuditTrailPageAsync(
            int? pageIndex,
            int? pageSize,
            string module,
            string entityType,
            string entityReference,
            string field,
            string action,
            string source,
            string updatedByUserId,
            DateTime? updatedFrom,
            DateTime? updatedTo,
            string value);

        Task<AdminSystemPagedListDto<SystemAuditValueHistoryRowDto>> GetAuditValueHistoryPageAsync(
            int? pageIndex,
            int? pageSize,
            string module,
            string entityType,
            string entityReference,
            string field,
            string value,
            DateTime? rangeFrom,
            DateTime? rangeTo);

        Task<AdminSystemPagedListDto<NotificationTransactionLogRowDto>> GetNotificationTransactionLogsPageAsync(
            int? pageIndex,
            int? pageSize,
            string channelType,
            string receiver,
            string actionCode,
            bool? isSuccess,
            DateTime? createdFrom,
            DateTime? createdTo);

        Task<NotificationTransactionLogRowDto> ResendNotificationTransactionAsync(long id);
    }
}
