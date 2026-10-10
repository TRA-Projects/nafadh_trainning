using System.Collections.Generic;

namespace Bidding_Models.Jobseekers
{
    public class CustomNotificationTargetDto
    {
        /// <summary><c>User</c>, <c>UserGroup</c>, <c>Organization</c>, <c>AllOrganizations</c> or <c>JobSeekers</c>.</summary>
        public string TargetType { get; set; }
        public string UserId { get; set; }
        public int? UserGroupId { get; set; }
        public int? OrganizationId { get; set; }
        public string Label { get; set; }
        /// <summary><c>All</c>, <c>Registered</c> or <c>NotRegistered</c> (JobSeekers / AllOrganizations only).</summary>
        public string RegistrationFilter { get; set; }
    }

    public class UpsertCustomNotificationRequestDto
    {
        public string Title_en { get; set; }
        public string Title_ar { get; set; }
        public string Body_en { get; set; }
        public string Body_ar { get; set; }
        public bool SendEmail { get; set; }
        public bool SendSms { get; set; }
        public bool SendSystem { get; set; }
        public bool SendNow { get; set; }
        /// <summary>ISO-8601 UTC; ignored when <see cref="SendNow"/> is true.</summary>
        public string ScheduledAtUtc { get; set; }
        public List<CustomNotificationTargetDto> Targets { get; set; } = new List<CustomNotificationTargetDto>();
    }

    public class CustomNotificationRowDto
    {
        public long Id { get; set; }
        public string Title_en { get; set; }
        public string Title_ar { get; set; }
        public bool SendEmail { get; set; }
        public bool SendSms { get; set; }
        public bool SendSystem { get; set; }
        public string ScheduledAtUtc { get; set; }
        public string Status { get; set; }
        public int TotalRecipients { get; set; }
        public int SentCount { get; set; }
        public int FailedCount { get; set; }
        public int SkippedCount { get; set; }
        public int UserTargetCount { get; set; }
        public int GroupTargetCount { get; set; }
        public int OrganizationTargetCount { get; set; }
        public bool AllOrganizations { get; set; }
        /// <summary>Registration filter of the "All organizations" target, when present.</summary>
        public string AllOrganizationsFilter { get; set; }
        /// <summary>Registration filter of the "Job seekers" target; null when that target is not selected.</summary>
        public string JobSeekersFilter { get; set; }
        public bool CanEdit { get; set; }
        public string CreatedAtUtc { get; set; }
        public string ProcessedAtUtc { get; set; }
    }

    public class CustomNotificationDto : CustomNotificationRowDto
    {
        public string Body_en { get; set; }
        public string Body_ar { get; set; }
        public List<CustomNotificationTargetDto> Targets { get; set; } = new List<CustomNotificationTargetDto>();
    }

    public class CustomNotificationRecipientPreviewDto
    {
        public int RegisteredUsers { get; set; }
        public int EmailOnlyOrganizations { get; set; }
        /// <summary>Imported job seekers with no account yet (reachable by email / SMS only).</summary>
        public int UnregisteredJobSeekers { get; set; }
        public int Total { get; set; }
    }

    public class UserNotificationDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Locale { get; set; }
        public bool IsRead { get; set; }
        public string CreatedAtUtc { get; set; }
        public string ReadAtUtc { get; set; }
    }

    public class UserNotificationUnreadCountDto
    {
        public int Count { get; set; }
    }
}
