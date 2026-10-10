using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.Employment
{
    [Table("ContractBenefitLines")]
    public class ContractBenefitLine
    {
        public int Id { get; set; }

        public int ContractId { get; set; }

        public EmploymentProcedureRecord Contract { get; set; }

        /// <summary>BENEF catalog value key.</summary>
        [Required]
        [MaxLength(64)]
        public string BenefitValueKey { get; set; }

        public string RemarksEn { get; set; }

        public string RemarksAr { get; set; }

        public bool IsPrivate { get; set; }

        /// <summary>Force-loaded from the related opportunity; UI must not delete.</summary>
        public bool FromOpportunity { get; set; }

        public int SortOrder { get; set; }
    }
}
