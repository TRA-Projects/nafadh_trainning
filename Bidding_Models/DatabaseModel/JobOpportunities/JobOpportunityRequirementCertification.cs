using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobOpportunities
{
    [Table("JobOpportunityRequirementCertifications")]
    public class JobOpportunityRequirementCertification
    {
        public int Id { get; set; }

        public int JobOpportunityRequirementId { get; set; }

        public JobOpportunityRequirement Requirement { get; set; }

        [Required]
        [MaxLength(512)]
        public string CertificationText { get; set; }

        public int SortOrder { get; set; }
    }
}
