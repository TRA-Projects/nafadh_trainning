using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.Credentials;
using Bidding_Models.DatabaseModel.Interviews;
using Bidding_Models.DatabaseModel.SystemCatalog;

namespace Bidding_Models.DatabaseModel.Assessments
{
    /// <summary>
    /// Per personal skill rating (merged <see cref="CatalogSkill"/> rows of kind Personal).
    /// Replaces fixed personal-evaluation columns; soft skills are catalog-driven.
    /// </summary>
    [Table("AssessmentBasicSkillRatings")]
    public class AssessmentBasicSkillRating
    {
        public int Id { get; set; }

        public int AssessmentResultId { get; set; }

        public AssessmentResult AssessmentResult { get; set; }

        public int CatalogBasicSkillId { get; set; }

        public CatalogSkill CatalogBasicSkill { get; set; }

        public int? InterviewRecordBasicSkillId { get; set; }

        public InterviewRecordBasicSkill InterviewRecordBasicSkill { get; set; }

        public int? CredentialDefinitionBasicSkillId { get; set; }

        public CredentialDefinitionBasicSkill CredentialDefinitionBasicSkill { get; set; }

        public decimal? Rating { get; set; }

        [MaxLength(32)]
        public string Qualitative { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }
    }
}
