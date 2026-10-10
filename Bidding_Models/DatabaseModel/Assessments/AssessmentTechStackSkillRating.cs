using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.Credentials;
using Bidding_Models.DatabaseModel.Interviews;
using Bidding_Models.DatabaseModel.SystemCatalog;

namespace Bidding_Models.DatabaseModel.Assessments
{
    [Table("AssessmentTechStackSkillRatings")]
    public class AssessmentTechStackSkillRating
    {
        public int Id { get; set; }

        public int AssessmentResultId { get; set; }

        public AssessmentResult AssessmentResult { get; set; }

        public int CatalogTechStackId { get; set; }

        public CatalogSkill CatalogTechStack { get; set; }

        public int? InterviewRecordTechStackId { get; set; }

        public InterviewRecordTechStack InterviewRecordTechStack { get; set; }

        public int? CredentialDefinitionTechStackId { get; set; }

        public CredentialDefinitionTechStack CredentialDefinitionTechStack { get; set; }

        public decimal? Rating { get; set; }

        [MaxLength(32)]
        public string Qualitative { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }
    }
}
