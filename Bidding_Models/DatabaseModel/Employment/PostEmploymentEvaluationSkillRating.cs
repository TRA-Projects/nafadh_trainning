using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.DatabaseModel.Employment
{
    /// <summary>
    /// Pre-migration entity for EvaluationSkillRatingDto rows belonging to a
    /// Final/Engagement evaluation. Not yet connected to CHDBContext.
    /// SkillId is string — matches EvaluationSkillRatingDto.SkillId and
    /// PostEmploymentSkillOptionDto.Id exactly; this is Post Employment's own
    /// self-contained skill catalog (mock codes "SK-01".."SK-04"), not the
    /// platform-wide int-keyed Skills entity.
    /// </summary>
    public class PostEmploymentEvaluationSkillRating
    {
        public int Id { get; set; }

        public int PostEmploymentEvaluationId { get; set; }

        public PostEmploymentEvaluation PostEmploymentEvaluation { get; set; }

        [Required]
        [MaxLength(32)]
        public string SkillId { get; set; }

        public decimal Rating { get; set; }
    }
}
