using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobOpportunities
{
    [Table("JobOpportunityApplicationMethods")]
    public class JobOpportunityApplicationMethod
    {
        public int Id { get; set; }

        public int JobOpportunityId { get; set; }

        public JobOpportunity JobOpportunity { get; set; }

        [Required]
        [MaxLength(64)]
        public string ApplicationMethodValueKey { get; set; }
    }
}
