using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobOpportunities
{
    [Table("JobOpportunities")]
    public class JobOpportunity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string JobReference { get; set; }

        public int OwnerProfileId { get; set; }

        public Profile OwnerProfile { get; set; }

        [Required]
        [MaxLength(512)]
        public string TitleEn { get; set; }

        [Required]
        [MaxLength(512)]
        public string TitleAr { get; set; }

        [MaxLength(256)]
        public string DepartmentEn { get; set; }

        [MaxLength(256)]
        public string DepartmentAr { get; set; }

        public string DescriptionEn { get; set; }

        public string DescriptionAr { get; set; }

        /// <summary>System config group <c>WRK_SCOPE</c> value key (opportunity field / work scope). Legacy jobs may still use <c>JOB_CAT</c> keys.</summary>
        [MaxLength(64)]
        public string CategoryValueKey { get; set; }

        public int Openings { get; set; }

        /// <summary>
        /// Cap on applications. Null or 0 = no cap.
        /// When applied count reaches this value, status becomes <see cref="JobOpportunityStatuses.UnderEvaluation"/>.
        /// </summary>
        public int? MaxApplicants { get; set; }

        public bool IsTra { get; set; }

        /// <summary>TRA program / nomination reference code when <see cref="IsTra"/> is true.</summary>
        [MaxLength(64)]
        public string Code { get; set; }

        [MaxLength(32)]
        public string NominationType { get; set; }

        [MaxLength(64)]
        public string JobTypeValueKey { get; set; }

        [MaxLength(64)]
        public string WorkModeValueKey { get; set; }

        /// <summary>System config group <c>TGT_GEND</c> value key.</summary>
        [MaxLength(64)]
        public string TargetGenderValueKey { get; set; }

        public int? MaleSeats { get; set; }

        public int? FemaleSeats { get; set; }

        [MaxLength(64)]
        public string InterviewModeValueKey { get; set; }

        [Required]
        [MaxLength(32)]
        public string Status { get; set; }

        public DateTime? PostedDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<JobOpportunityApplicationMethod> ApplicationMethods { get; set; } =
            new List<JobOpportunityApplicationMethod>();

        public List<JobOpportunityAttachment> Attachments { get; set; } = new List<JobOpportunityAttachment>();

        public List<JobOpportunityParticipation> Participations { get; set; } = new List<JobOpportunityParticipation>();
    }
}
