using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.Credentials;
using Bidding_Models.DatabaseModel.Interviews;

namespace Bidding_Models.DatabaseModel.Assessments
{
    /// <summary>
    /// Overall result for one assessment submit (credential, interview, or employer periodic review).
    /// Per-skill lines live in the three <c>Assessment*SkillRating</c> tables.
    /// Lifetime rollup lives in <see cref="JobSeekerSkillSummary"/>.
    /// </summary>
    [Table("AssessmentResults")]
    public class AssessmentResult
    {
        public int Id { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; }

        /// <summary><see cref="AssessmentSourceTypes"/> value.</summary>
        [Required]
        [MaxLength(32)]
        public string SourceType { get; set; }

        public int? CredentialEnrollmentId { get; set; }

        public CredentialEnrollment CredentialEnrollment { get; set; }

        public int? InterviewRecordId { get; set; }

        public InterviewRecord InterviewRecord { get; set; }

        /// <summary>Operator/evaluator profile that submitted a periodic rating (when applicable).</summary>
        public int? SubmittedByProfileId { get; set; }

        /// <summary>Snapshotted from credential definition, interview, or system variable at submit.</summary>
        public int Weight { get; set; }

        /// <summary><see cref="AssessmentResultStatuses"/> value.</summary>
        [Required]
        [MaxLength(32)]
        public string Status { get; set; }

        /// <summary>Overall score for this single event (product scale, typically 1–5 or 0–100).</summary>
        public decimal? OverallScore { get; set; }

        public string Recommendations { get; set; }

        public string Feedback { get; set; }

        /// <summary>Employer periodic: points of strength.</summary>
        public string Strengths { get; set; }

        /// <summary>Employer periodic: areas for improvement.</summary>
        public string AreasForImprovement { get; set; }

        /// <summary>Employer periodic: operator-internal remarks.</summary>
        public string OperatorNotes { get; set; }

        [MaxLength(32)]
        public string FinalDecision { get; set; }

        /// <summary>
        /// System config <see cref="SystemCatalog.SystemConfigValue.ValueKey"/> (e.g. interview rejection main reason).
        /// Same catalog/group semantics as <see cref="SubRejectionReasonValueKey"/>.
        /// </summary>
        [MaxLength(64)]
        public string RejectionReasonValueKey { get; set; }

        /// <summary>
        /// Child system config value key; <see cref="SystemCatalog.SystemConfigValue.ParentValueKey"/> references
        /// <see cref="RejectionReasonValueKey"/>.
        /// </summary>
        [MaxLength(64)]
        public string SubRejectionReasonValueKey { get; set; }

        public DateTime CompletionDate { get; set; }

        public DateTime? RescheduleDate { get; set; }

        public List<AssessmentProfessionalSkillRating> ProfessionalSkillRatings { get; set; } =
            new List<AssessmentProfessionalSkillRating>();

        public List<AssessmentBasicSkillRating> BasicSkillRatings { get; set; } =
            new List<AssessmentBasicSkillRating>();

        public List<AssessmentTechStackSkillRating> TechStackSkillRatings { get; set; } =
            new List<AssessmentTechStackSkillRating>();

        public List<AssessmentResultBehavior> Behaviors { get; set; } = new List<AssessmentResultBehavior>();
    }
}
