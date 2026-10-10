using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.Notifications
{
    /// <summary>Admin-authored notification delivered by email, SMS and/or the in-app inbox at a scheduled time.</summary>
    [Table("CustomNotifications")]
    public class CustomNotification
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Title_en { get; set; }

        [Required]
        [MaxLength(256)]
        public string Title_ar { get; set; }

        [Required]
        public string Body_en { get; set; }

        [Required]
        public string Body_ar { get; set; }

        public bool SendEmail { get; set; }

        public bool SendSms { get; set; }

        public bool SendSystem { get; set; }

        public DateTime ScheduledAtUtc { get; set; }

        /// <summary>See <c>CustomNotificationStatuses</c>.</summary>
        [Required]
        [MaxLength(32)]
        public string Status { get; set; }

        public int TotalRecipients { get; set; }

        public int SentCount { get; set; }

        public int FailedCount { get; set; }

        public int SkippedCount { get; set; }

        [MaxLength(450)]
        public string CreatedByUserId { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public DateTime? ProcessedAtUtc { get; set; }

        public List<CustomNotificationTarget> Targets { get; set; } = new List<CustomNotificationTarget>();
    }
}
