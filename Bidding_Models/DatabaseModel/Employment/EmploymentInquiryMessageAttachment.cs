using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.JobseekerProfile;

namespace Bidding_Models.DatabaseModel.Employment
{
    [Table("EmploymentInquiryMessageAttachments")]
    public class EmploymentInquiryMessageAttachment
    {
        public int Id { get; set; }

        public int EmploymentInquiryMessageId { get; set; }

        public EmploymentInquiryMessage EmploymentInquiryMessage { get; set; }

        public int AttachmentId { get; set; }

        public Attachment Attachment { get; set; }
    }
}
