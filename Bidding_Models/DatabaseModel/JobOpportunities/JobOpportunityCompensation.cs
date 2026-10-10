using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobOpportunities
{
    [Table("JobOpportunityCompensations")]
    public class JobOpportunityCompensation
    {
        public int Id { get; set; }

        public int JobOpportunityId { get; set; }

        public JobOpportunity JobOpportunity { get; set; }

        public string SalaryDisplayValueKey { get; set; }

        public decimal SalaryMin { get; set; }

        public decimal SalaryMax { get; set; }

        public string CurrencyValueKey { get; set; }

        public string PayFrequencyValueKey { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? Deadline { get; set; }

        public decimal? Budget { get; set; }

        public string BudgetType { get; set; }

        public string ProjectEstimation { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<JobOpportunityCompensationBenefit> Benefits { get; set; } =
            new List<JobOpportunityCompensationBenefit>();
    }
}
