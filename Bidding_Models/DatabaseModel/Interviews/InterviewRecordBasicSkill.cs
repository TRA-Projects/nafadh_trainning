using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.SystemCatalog;

namespace Bidding_Models.DatabaseModel.Interviews
{
    [Table("InterviewRecordBasicSkills")]
    public class InterviewRecordBasicSkill
    {
        public int Id { get; set; }

        public int InterviewRecordId { get; set; }

        public InterviewRecord InterviewRecord { get; set; }

        /// <summary>FK into the merged <see cref="CatalogSkill"/> table (rows with <see cref="CatalogSkillKind.Personal"/>).</summary>
        public int CatalogBasicSkillId { get; set; }

        public CatalogSkill CatalogBasicSkill { get; set; }
    }
}
