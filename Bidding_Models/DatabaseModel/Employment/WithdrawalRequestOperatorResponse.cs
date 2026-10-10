using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.Employment
{
    [Table("WithdrawalRequestOperatorResponses")]
    public class WithdrawalRequestOperatorResponse
    {
        public int Id { get; set; }

        public int WithdrawalRequestId { get; set; }

        public WithdrawalRequest WithdrawalRequest { get; set; }

        [Required]
        [MaxLength(32)]
        public string Decision { get; set; }

        public string Notes { get; set; }

        [MaxLength(64)]
        public string RejectionReason { get; set; }

        [MaxLength(64)]
        public string SubRejectionReason { get; set; }

        public string InformationQuestion { get; set; }

        public DateTime RespondedAt { get; set; }

        public int? RespondedByOperatorProfileId { get; set; }

        public Profile RespondedByOperatorProfile { get; set; }
    }
}
