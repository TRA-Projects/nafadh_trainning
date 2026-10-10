using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.SystemCatalog;

namespace Bidding_Models.DatabaseModel.Interviews
{
    [Table("InterviewRecordSkills")]
    public class InterviewRecordSkill
    {
        public int Id { get; set; }

        public int InterviewRecordId { get; set; }

        public InterviewRecord InterviewRecord { get; set; }

        public int CatalogSkillId { get; set; }

        public CatalogSkill CatalogSkill { get; set; }
    }
}
