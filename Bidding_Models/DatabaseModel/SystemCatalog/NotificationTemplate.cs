using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.SystemCatalog
{
    /// <summary>
    /// Bilingual notification template keyed by stable <see cref="ActionCode"/> (one row per action).
    /// </summary>
    [Table("NotificationTemplates")]
    public class NotificationTemplate
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string ActionCode { get; set; }

        [Required]
        [MaxLength(256)]
        public string LabelEn { get; set; }

        [Required]
        [MaxLength(256)]
        public string LabelAr { get; set; }

        [MaxLength(512)]
        public string EmailSubjectEn { get; set; }

        [MaxLength(512)]
        public string EmailSubjectAr { get; set; }

        public string EmailBodyEn { get; set; }

        public string EmailBodyAr { get; set; }

        [MaxLength(500)]
        public string SmsBodyEn { get; set; }

        [MaxLength(500)]
        public string SmsBodyAr { get; set; }

        public bool IsActive { get; set; } = true;

        public bool EmailEnabled { get; set; } = true;

        public bool SmsEnabled { get; set; } = true;

        public int SortOrder { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
