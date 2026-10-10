using System;
using System.ComponentModel.DataAnnotations;
using Bidding_Models.DatabaseModel.Assessments;

namespace Bidding_Models.DatabaseModel.Employment
{
    /// <summary>
    /// Slim workflow row for employer periodic reviews (period window / due / acknowledge).
    /// Ratings and narrative live on the linked <see cref="AssessmentResult"/>.
    /// </summary>
    public class PostEmploymentPeriodicEvaluation
    {
        public int Id { get; set; }

        public int PostEmploymentRecordId { get; set; }

        public PostEmploymentRecord PostEmploymentRecord { get; set; }

        [Required]
        [MaxLength(16)]
        public string Period { get; set; }

        public DateTime PeriodFrom { get; set; }

        public DateTime PeriodTo { get; set; }

        public int EvaluationMonthIndex { get; set; }

        public DateTime? DueDate { get; set; }

        [Required]
        [MaxLength(16)]
        public string Status { get; set; }

        public bool IsDraft { get; set; }

        public int? AssessmentResultId { get; set; }

        public AssessmentResult AssessmentResult { get; set; }

        public int? SubmittedByProfileId { get; set; }

        public DateTime? SubmittedAt { get; set; }

        public DateTime? AcknowledgedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
