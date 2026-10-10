using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobOpportunities
{
    [Table("JobOpportunityRequirementLanguages")]
    public class JobOpportunityRequirementLanguage
    {
        public int Id { get; set; }

        public int JobOpportunityRequirementId { get; set; }

        public JobOpportunityRequirement Requirement { get; set; }

        [Required]
        [MaxLength(64)]
        public string LanguageValueKey { get; set; }
    }
}
