using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.SystemCatalog;

namespace Bidding_Models.DatabaseModel.Assessments
{
    /// <summary>
    /// Standalone accumulated rating for one jobseeker on one catalog skill (any of the three kinds).
    /// Updated when a submitted <see cref="AssessmentResult"/> is saved.
    /// </summary>
    [Table("JobSeekerSkillSummaries")]
    public class JobSeekerSkillSummary
    {
        public int Id { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; }

        /// <summary><see cref="AssessmentSkillKinds"/> value.</summary>
        [Required]
        [MaxLength(16)]
        public string SkillKind { get; set; }

        public int? CatalogSkillId { get; set; }

        public CatalogSkill CatalogSkill { get; set; }

        /// <summary>FK into the merged <see cref="CatalogSkill"/> table (rows with <see cref="CatalogSkillKind.Personal"/>).</summary>
        public int? CatalogBasicSkillId { get; set; }

        public CatalogSkill CatalogBasicSkill { get; set; }

        public int? CatalogTechStackId { get; set; }

        public CatalogSkill CatalogTechStack { get; set; }

        public decimal AccumulatedScore { get; set; }

        public int AssessmentCount { get; set; }

        public int InterviewCount { get; set; }

        /// <summary>Count of contributing <see cref="AssessmentSourceTypes.PeriodicRating"/> results.</summary>
        public int PeriodicRatingCount { get; set; }

        public int? LastAssessmentResultId { get; set; }

        public AssessmentResult LastAssessmentResult { get; set; }

        public DateTime? LastRatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
