using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel
{
    public class Engagement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EngagementProfileId { get; set; }

        [Required]
        public int ProjectId { get; set; }
        
        [Required]
        public int ProjectProfileId { get; set; }
        
        [Required]
        public Enums.EngagementType Type { get; set; }
        
        [Required]
        public DateTime SubmissionDate { get; set; }

        [Required]
        public Enums.EngagementStatus Status { get; set; }

        public string ChangeStatusRemark { get; set; }
        public string Designation { get; set; }
        public string ResumeUrl { get; set; }
        public string CoverLetterUrl { get; set; }
        public string AdditionalFileUrl { get; set; }
        
        public decimal? BidAmount { get; set; }
        //timeline 3 weeks  or 2 years 
        public TimeSpan? Timeline { get; set; }
        public string PricingSelection { get; set; }
        public int EstimationToCompleteTheProject { get; set; }
        public string CoverLetter { get; set; }
        public string WorkPlan { get; set; }
        public bool IsDigitalMarketplace { get; set; }

        public decimal AnswersEvaluate { get; set; }
        public ICollection<EngagementAttachment> Attachments { get; set; } = new List<EngagementAttachment>();
        public ICollection<EngagementAudit> AuditLogs { get; set; }


    }
    public class EngagementAttachment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EngagementId { get; set; }

        [Required, MaxLength(50)]
        public string AttachmentType { get; set; }

        [Required]
        public string AttachmentUrl { get; set; }

        [Required, MaxLength(255)]
        public string AttachmentName { get; set; }
    }
    public class EngagementAudit
    {
        public int Id { get; set; }
        public int EngagementId { get; set; }
        public int? ActionMadeByProfileId { get; set; }
        public DateTime ActionTime { get; set; }
        public string ActionTitle { get; set; }
        public string ActionTitleArabic { get; set; }
        //public Profile ActionMadeByProfile { get; set; }
    }
}