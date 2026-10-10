using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobOpportunities
{
    [Table("JobOpportunityRequirementFieldOfStudies")]
    public class JobOpportunityRequirementFieldOfStudy
    {
        public int Id { get; set; }

        public int JobOpportunityRequirementId { get; set; }

        public JobOpportunityRequirement Requirement { get; set; }

        [Required]
        [MaxLength(64)]
        public string FieldOfStudyValueKey { get; set; }
    }
}
