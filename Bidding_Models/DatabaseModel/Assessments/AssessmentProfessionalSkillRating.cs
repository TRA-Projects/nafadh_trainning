using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.Credentials;
using Bidding_Models.DatabaseModel.Interviews;
using Bidding_Models.DatabaseModel.SystemCatalog;

namespace Bidding_Models.DatabaseModel.Assessments
{
    [Table("AssessmentProfessionalSkillRatings")]
    public class AssessmentProfessionalSkillRating
    {
        public int Id { get; set; }

        public int AssessmentResultId { get; set; }

        public AssessmentResult AssessmentResult { get; set; }

        public int CatalogSkillId { get; set; }

        public CatalogSkill CatalogSkill { get; set; }

        public int? InterviewRecordSkillId { get; set; }

        public InterviewRecordSkill InterviewRecordSkill { get; set; }

        public int? CredentialDefinitionSkillId { get; set; }

        public CredentialDefinitionSkill CredentialDefinitionSkill { get; set; }

        public decimal? Rating { get; set; }

        [MaxLength(32)]
        public string Qualitative { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }
    }
}
