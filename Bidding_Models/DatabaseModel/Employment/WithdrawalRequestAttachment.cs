using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.JobseekerProfile;

namespace Bidding_Models.DatabaseModel.Employment
{
    [Table("WithdrawalRequestAttachments")]
    public class WithdrawalRequestAttachment
    {
        public int Id { get; set; }

        public int WithdrawalRequestId { get; set; }

        public WithdrawalRequest WithdrawalRequest { get; set; }

        public int AttachmentId { get; set; }

        public Attachment Attachment { get; set; }
    }
}
