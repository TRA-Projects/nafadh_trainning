using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobOpportunities
{
    [Table("JobOpportunityScreeningQuestions")]
    public class JobOpportunityScreeningQuestion
    {
        public int Id { get; set; }

        public int JobOpportunityId { get; set; }

        public JobOpportunity JobOpportunity { get; set; }

        [Required]
        public string QuestionEn { get; set; }

        public string QuestionAr { get; set; }

        [Required]
        [MaxLength(64)]
        public string QuestionType { get; set; }

        [MaxLength(32)]
        public string Importance { get; set; }

        public int SortOrder { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<JobOpportunityScreeningQuestionOption> Options { get; set; } =
            new List<JobOpportunityScreeningQuestionOption>();
    }
}
