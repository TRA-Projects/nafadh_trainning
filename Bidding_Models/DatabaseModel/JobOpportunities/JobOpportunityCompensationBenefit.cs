using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobOpportunities
{
    [Table("JobOpportunityCompensationBenefits")]
    public class JobOpportunityCompensationBenefit
    {
        public int Id { get; set; }

        public int JobOpportunityCompensationId { get; set; }

        public JobOpportunityCompensation Compensation { get; set; }

        [Required]
        [MaxLength(64)]
        public string BenefitValueKey { get; set; }
    }
}
