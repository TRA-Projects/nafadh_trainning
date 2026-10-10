using System.Collections.Generic;
using Bidding_Models.DatabaseModel.SystemCatalog;

namespace Bidding_Models.Jobseekers
{
    public class AdminSystemPagedListDto<T> : PagedListResponseDto<T>
    {
        public int pageSize { get; set; }
    }

    public class SystemVariableDto
    {
        /// <summary>Programmatic lookup key (same as <see cref="VariableKey"/>).</summary>
        public string Code { get; set; }
        public string VariableKey { get; set; }
        public string VariableName { get; set; }
        public string VariableNameEn { get; set; }
        public string VariableNameAr { get; set; }
        public string ValueType { get; set; }
        public object Value { get; set; }
        public string Description { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
    }

    public class UpdateSystemVariableRequestDto
    {
        public object Value { get; set; }
    }

    public class SystemConfigValueDto
    {
        public object id { get; set; }
        public string code { get; set; }
        public string label_en { get; set; }
        public string label_ar { get; set; }
        public bool IsActive { get; set; } = true;
        public bool? subReasonDropdown { get; set; }
        public object parentReasonId { get; set; }
    }

    public class CreateSystemConfigValueRequestDto
    {
        public string code { get; set; }
        public string label_en { get; set; }
        public string label_ar { get; set; }
        public bool IsActive { get; set; } = true;
        public bool? subReasonDropdown { get; set; }
        public object parentReasonId { get; set; }
    }

    public class UpdateSystemConfigValueRequestDto
    {
        public string label_en { get; set; }
        public string label_ar { get; set; }
        public bool IsActive { get; set; } = true;
        public bool? subReasonDropdown { get; set; }
        public object parentReasonId { get; set; }
    }

    public class EnsureSystemConfigValueRequestDto
    {
        public string label_en { get; set; }
        public string label_ar { get; set; }
        public bool IsActive { get; set; } = true;
        public object parentReasonId { get; set; }
    }

    public class SkillCategoryDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class SkillDto
    {
        public int Id { get; set; }
        /// <summary>Null for tech-stack rows (flat list without categories).</summary>
        public int? CategoryId { get; set; }
        public CatalogSkillKind Kind { get; set; } = CatalogSkillKind.Professional;
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public bool IsActive { get; set; }
    }

    public class SystemAuditTrailRowDto
    {
        public long Id { get; set; }
        public string Module { get; set; }
        public string EntityType { get; set; }
        public string EntityReference { get; set; }
        public string Field { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Action { get; set; }
        public string Source { get; set; }
        public string UpdatedByUserId { get; set; }
        public string UpdatedDate { get; set; }
    }

    public class SystemAuditValueHistoryRowDto
    {
        public string Module { get; set; }
        public string EntityType { get; set; }
        public string EntityReference { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
        public string ValidFrom { get; set; }
        public string ValidTo { get; set; }
        public string UpdatedByUserId { get; set; }
        public string Source { get; set; }
    }

    public class NotificationTransactionLogRowDto
    {
        public long Id { get; set; }
        /// <summary><c>Email</c> or <c>SMS</c>.</summary>
        public string ChannelType { get; set; }
        public string Receiver { get; set; }
        public string Subject { get; set; }
        public string MessageBody { get; set; }
        public string PreferredLanguage { get; set; }
        public string ActionCode { get; set; }
        public string ActionLabelEn { get; set; }
        public string ActionLabelAr { get; set; }
        public int? ProfileId { get; set; }
        public bool IsSuccess { get; set; }
        public string FailureReason { get; set; }
        public string CreatedAtUtc { get; set; }
    }
}
