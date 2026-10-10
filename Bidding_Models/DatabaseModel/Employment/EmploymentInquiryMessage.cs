using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.Employment
{
    [Table("EmploymentInquiryMessages")]
    public class EmploymentInquiryMessage
    {
        public int Id { get; set; }

        public int EmploymentProcedureRecordId { get; set; }

        public EmploymentProcedureRecord EmploymentProcedureRecord { get; set; }

        [Required]
        [MaxLength(16)]
        public string SenderType { get; set; }

        [Required]
        public string MessageEn { get; set; }

        [Required]
        public string MessageAr { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<EmploymentInquiryMessageAttachment> Attachments { get; set; } =
            new List<EmploymentInquiryMessageAttachment>();
    }
}
