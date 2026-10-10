using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.Assessments
{
    /// <summary>
    /// One observed behavior on an assessment result (system config group <c>BEHV</c>).
    /// </summary>
    [Table("AssessmentResultBehaviors")]
    public class AssessmentResultBehavior
    {
        public int Id { get; set; }

        public int AssessmentResultId { get; set; }

        public AssessmentResult AssessmentResult { get; set; }

        /// <summary><see cref="SystemCatalog.SystemConfigValue.ValueKey"/> in the behavior catalog.</summary>
        [Required]
        [MaxLength(AssessmentBehaviorKeys.MaxLength)]
        public string ValueKey { get; set; }
    }
}
