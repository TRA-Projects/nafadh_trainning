using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobOpportunities
{
    /// <summary>One row per profile + opportunity (nominate, ask-to-apply, apply, pipeline).</summary>
    [Table("JobOpportunityParticipations")]
    public class JobOpportunityParticipation
    {
        public int Id { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; }

        public int JobOpportunityId { get; set; }

        public JobOpportunity JobOpportunity { get; set; }

        public int? MatchPercent { get; set; }

        public bool IsTraNominated { get; set; }

        public bool IsNominated { get; set; }

        public bool AskToApplySent { get; set; }

        /// <summary>
        /// System-config value key (<c>ATA_RJCT</c>) when the job seeker declined the ask-to-apply invitation.
        /// </summary>
        [MaxLength(64)]
        public string AskToApplyRejectionReasonValueKey { get; set; }

        public bool HasApplied { get; set; }

        public bool IsShortlisted { get; set; }

        [MaxLength(32)]
        public string PipelineStatus { get; set; }

        [MaxLength(32)]
        public string ApplicationType { get; set; }

        public bool NominatedByTra { get; set; }

        [MaxLength(64)]
        public string StudentCode { get; set; }

        public DateTime? AppliedOn { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<JobOpportunityParticipationAnswer> Answers { get; set; } =
            new List<JobOpportunityParticipationAnswer>();

        public List<JobOpportunityParticipationAttachment> Attachments { get; set; } =
            new List<JobOpportunityParticipationAttachment>();
    }
}
