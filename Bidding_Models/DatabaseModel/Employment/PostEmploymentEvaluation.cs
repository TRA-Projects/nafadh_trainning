using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.DatabaseModel.Employment
{
    /// <summary>
    /// Pre-migration entity for the existing Final/Engagement evaluation model
    /// (SaveEvaluationBodyDto / EngagementEvaluationDto). Not yet connected to
    /// CHDBContext — see the Evaluation migration handover spec before wiring
    /// DbSets, Fluent API mappings, or repository queries against this table.
    /// </summary>
    public class PostEmploymentEvaluation
    {
        public int Id { get; set; }

        public int PostEmploymentRecordId { get; set; }

        public PostEmploymentRecord PostEmploymentRecord { get; set; }

        public decimal OverallRating { get; set; }

        [Required]
        [MaxLength(16)]
        public string Status { get; set; }

        [MaxLength(32)]
        public string Outcome { get; set; }

        public string NotesEn { get; set; }

        public string NotesAr { get; set; }

        public DateTime? EvaluationDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<PostEmploymentEvaluationSkillRating> SkillRatings { get; set; } =
            new List<PostEmploymentEvaluationSkillRating>();
    }
}
