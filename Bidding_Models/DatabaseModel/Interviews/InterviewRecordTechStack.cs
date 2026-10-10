using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.SystemCatalog;

namespace Bidding_Models.DatabaseModel.Interviews
{
    [Table("InterviewRecordTechStacks")]
    public class InterviewRecordTechStack
    {
        public int Id { get; set; }

        public int InterviewRecordId { get; set; }

        public InterviewRecord InterviewRecord { get; set; }

        public int CatalogTechStackId { get; set; }

        public CatalogSkill CatalogTechStack { get; set; }
    }
}
