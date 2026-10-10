using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobOpportunities
{
    [Table("JobSeekerSavedOpportunities")]
    public class JobSeekerSavedOpportunity
    {
        public int Id { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; }

        public int JobOpportunityId { get; set; }

        public JobOpportunity JobOpportunity { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
