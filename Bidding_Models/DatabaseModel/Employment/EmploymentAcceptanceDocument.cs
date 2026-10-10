using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.JobseekerProfile;

namespace Bidding_Models.DatabaseModel.Employment
{
    [Table("EmploymentAcceptanceDocuments")]
    public class EmploymentAcceptanceDocument
    {
        public int Id { get; set; }

        public int EmploymentProcedureRecordId { get; set; }

        public EmploymentProcedureRecord EmploymentProcedureRecord { get; set; }

        [Required]
        [MaxLength(512)]
        public string TitleEn { get; set; }

        [Required]
        [MaxLength(512)]
        public string TitleAr { get; set; }

        public string DescriptionEn { get; set; }

        public string DescriptionAr { get; set; }

        public int? AttachmentId { get; set; }

        public Attachment Attachment { get; set; }

        public bool IsRequired { get; set; }

        public int SortOrder { get; set; }
    }
}
