using System.Collections.Generic;

namespace Bidding_Models.Jobseekers
{
    /// <summary>Shared skill link shape for credential definitions and interview records.</summary>
    public class AssessmentSkillLinksDto
    {
        public List<string> associatedSkillIds { get; set; } = new List<string>();
        public List<string> associatedBasicSkillIds { get; set; } = new List<string>();
        public List<string> associatedTechStackIds { get; set; } = new List<string>();
    }

    /// <summary>One rated skill line across professional, basic, or tech stack catalogs.</summary>
    public class AssessmentSkillRatingDto
    {
        /// <summary><see cref="AssessmentSkillRatingKinds"/> value.</summary>
        public string skillKind { get; set; }

        /// <summary>Catalog id as string (professional / personal / tech-stack CatalogSkill rows).</summary>
        public string catalogId { get; set; }

        public decimal? rating { get; set; }

        public string qualitative { get; set; }

        public string notes { get; set; }

        /// <summary>Optional junction row id when rating a specific interview/credential skill link.</summary>
        public int? contextSkillLinkId { get; set; }
    }

    public static class AssessmentSkillRatingKinds
    {
        public const string Professional = "Professional";
        /// <summary>Personal skills (catalog <c>CatalogSkillKind.Personal</c>); persisted in AssessmentBasicSkillRatings.</summary>
        public const string Basic = "Basic";
        public const string TechStack = "TechStack";
    }

    /// <summary>
    /// Part 1 — overall assessment result (credential or interview).
    /// Per-skill lines are in <see cref="skillRatings"/>.
    /// </summary>
    public class AssessmentResultDto
    {
        public string sourceType { get; set; }

        public int profileId { get; set; }

        public string credentialEnrollmentReference { get; set; }

        public string interviewRecordReference { get; set; }

        /// <summary>Existing AssessmentResults.Id when updating a periodic rating draft.</summary>
        public int? assessmentResultId { get; set; }

        public int? submittedByProfileId { get; set; }

        public int weight { get; set; }

        public string status { get; set; }

        public decimal? overallScore { get; set; }

        public string recommendations { get; set; }

        public string feedback { get; set; }

        public string strengths { get; set; }

        public string areasForImprovement { get; set; }

        public string operatorNotes { get; set; }

        public string finalDecision { get; set; }

        /// <summary>System config value key (main rejection reason).</summary>
        public string rejectionReasonValueKey { get; set; }

        /// <summary>System config value key (sub rejection reason; parent is main reason).</summary>
        public string subRejectionReasonValueKey { get; set; }

        public string completionDateIso { get; set; }

        public string rescheduleDateIso { get; set; }

        public List<AssessmentSkillRatingDto> skillRatings { get; set; } = new List<AssessmentSkillRatingDto>();

        /// <summary>
        /// System config value keys (group <c>BEHV</c>). Null leaves stored behaviors unchanged.
        /// </summary>
        public List<string> behaviorValueKeys { get; set; }
    }

    /// <summary>Part 3 — accumulated jobseeker skill rollup.</summary>
    public class JobSeekerSkillSummaryDto
    {
        public string skillKind { get; set; }

        public string catalogId { get; set; }

        public decimal accumulatedScore { get; set; }

        public int assessmentCount { get; set; }

        public int interviewCount { get; set; }

        public int periodicRatingCount { get; set; }

        public string lastRatedAtIso { get; set; }
    }

    /// <summary>One distinct profile skill available for self-rating (with optional saved score).</summary>
    public class JobSeekerSelfRatingSkillDto
    {
        public string skillKind { get; set; }

        public string catalogId { get; set; }

        public string nameEn { get; set; }

        public string nameAr { get; set; }

        public decimal? rating { get; set; }
    }

    /// <summary>Profile-completion Rating tab payload.</summary>
    public class JobSeekerSelfRatingPageDto
    {
        public bool enabled { get; set; }

        /// <summary>
        /// Current live <c>SELF_RATING_WEIGHT</c> system variable (admin-editable).
        /// Not the value stored on <c>AssessmentResults.Weight</c> (that stays 0 for self-rating).
        /// </summary>
        public int weight { get; set; }

        public List<JobSeekerSelfRatingSkillDto> skills { get; set; } = new List<JobSeekerSelfRatingSkillDto>();
    }

    /// <summary>Save body for jobseeker self-rating.</summary>
    public class JobSeekerSelfRatingSaveDto
    {
        public List<AssessmentSkillRatingDto> skillRatings { get; set; } = new List<AssessmentSkillRatingDto>();
    }
}
