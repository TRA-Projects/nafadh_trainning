using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.Employment
{
    /// <summary>
    /// Post-employment contract change history (terminated, extended, type changed, etc.).
    /// </summary>
    [Table("EmploymentContractChangeHistory")]
    public class EmploymentContractChangeHistory
    {
        public int Id { get; set; }

        [MaxLength(64)]
        public string Reference { get; set; }

        public int PostEmploymentRecordId { get; set; }

        public PostEmploymentRecord PostEmploymentRecord { get; set; }

        public int EmploymentProcedureRecordId { get; set; }

        public EmploymentProcedureRecord EmploymentProcedureRecord { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; }

        [Required]
        [MaxLength(64)]
        public string ChangeType { get; set; }

        [MaxLength(64)]
        public string Reason { get; set; }

        [MaxLength(64)]
        public string SubReason { get; set; }

        public string NotesEn { get; set; }

        public string NotesAr { get; set; }

        public DateTime EffectiveDate { get; set; }

        [MaxLength(32)]
        public string InitiatedBy { get; set; }

        public bool IsVoided { get; set; }

        [MaxLength(64)]
        public string NewContractType { get; set; }

        public DateTime? NewContractStartDate { get; set; }

        public DateTime? NewContractEndDate { get; set; }

        [MaxLength(8)]
        public string NewCurrencyCode { get; set; }

        [MaxLength(64)]
        public string NewPayFrequency { get; set; }

        public decimal? NewAmount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
