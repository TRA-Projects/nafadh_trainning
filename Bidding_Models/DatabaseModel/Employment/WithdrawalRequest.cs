using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.Employment
{
    /// <summary>Jobseeker-submitted request with a snapshot of contract details at submission time.</summary>
    [Table("WithdrawalRequests")]
    public class WithdrawalRequest
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string Reference { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; }

        public int EmploymentProcedureRecordId { get; set; }

        public EmploymentProcedureRecord EmploymentProcedureRecord { get; set; }

        public int? PostEmploymentRecordId { get; set; }

        public PostEmploymentRecord PostEmploymentRecord { get; set; }

        [Required]
        [MaxLength(32)]
        public string RequestType { get; set; }

        [Required]
        [MaxLength(32)]
        public string Status { get; set; }

        [MaxLength(64)]
        public string ReasonCategory { get; set; }

        public string Reason { get; set; }

        public string JobseekerNote { get; set; }

        [MaxLength(64)]
        public string SnapshotContractType { get; set; }

        [MaxLength(8)]
        public string SnapshotCurrencyCode { get; set; }

        [MaxLength(64)]
        public string SnapshotPayFrequency { get; set; }

        public decimal? SnapshotAmount { get; set; }

        public DateTime? SnapshotContractStartDate { get; set; }

        public DateTime? SnapshotContractEndDate { get; set; }

        [MaxLength(32)]
        public string SnapshotContractStatus { get; set; }

        public DateTime SubmittedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<WithdrawalRequestAttachment> Attachments { get; set; } =
            new List<WithdrawalRequestAttachment>();

        public WithdrawalRequestOperatorResponse OperatorResponse { get; set; }
    }
}
