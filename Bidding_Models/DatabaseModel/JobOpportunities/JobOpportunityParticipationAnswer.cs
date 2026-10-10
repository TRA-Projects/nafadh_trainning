using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobOpportunities
{
    [Table("JobOpportunityParticipationAnswers")]
    public class JobOpportunityParticipationAnswer
    {
        public int Id { get; set; }

        public int ParticipationId { get; set; }

        public JobOpportunityParticipation Participation { get; set; }

        public int? ScreeningQuestionId { get; set; }

        public string QuestionEn { get; set; }

        public string QuestionAr { get; set; }

        public string AnswerText { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
