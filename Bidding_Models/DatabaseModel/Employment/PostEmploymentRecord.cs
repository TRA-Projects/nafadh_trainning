using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.Employment
{
    [Table("PostEmploymentRecords")]
    public class PostEmploymentRecord
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string Reference { get; set; }

        public int EmploymentProcedureRecordId { get; set; }

        public EmploymentProcedureRecord EmploymentProcedureRecord { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; }

        [Required]
        [MaxLength(32)]
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<EmploymentContractChangeHistory> ContractChangeHistory { get; set; } =
            new List<EmploymentContractChangeHistory>();

        public List<WithdrawalRequest> WithdrawalRequests { get; set; } = new List<WithdrawalRequest>();

        public PostEmploymentEvaluation Evaluation { get; set; }

        public List<PostEmploymentPeriodicEvaluation> PeriodicEvaluations { get; set; } =
            new List<PostEmploymentPeriodicEvaluation>();
    }
}
