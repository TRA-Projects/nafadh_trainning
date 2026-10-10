using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobOpportunities
{
    [Table("JobOpportunityScreeningQuestionOptions")]
    public class JobOpportunityScreeningQuestionOption
    {
        public int Id { get; set; }

        public int ScreeningQuestionId { get; set; }

        public JobOpportunityScreeningQuestion ScreeningQuestion { get; set; }

        [Required]
        [MaxLength(512)]
        public string AnswerEn { get; set; }

        [MaxLength(512)]
        public string AnswerAr { get; set; }

        public bool IsCorrect { get; set; }

        public int SortOrder { get; set; }
    }
}
