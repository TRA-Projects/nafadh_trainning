using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.Employment
{
    [Table("ContractWageLines")]
    public class ContractWageLine
    {
        public int Id { get; set; }

        public int ContractId { get; set; }

        public EmploymentProcedureRecord Contract { get; set; }

        /// <summary>ALLOW catalog value key (e.g. basic-salary / BASIC).</summary>
        [Required]
        [MaxLength(64)]
        public string AllowanceValueKey { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal Amount { get; set; }

        public int SortOrder { get; set; }

        /// <summary>True for the mandatory basic-salary line (not deletable).</summary>
        public bool IsLocked { get; set; }
    }
}
