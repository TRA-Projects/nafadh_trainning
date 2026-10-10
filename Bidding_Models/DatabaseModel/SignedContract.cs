using System;
using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.DatabaseModel
{
    public class SignedContract
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }

        [Required]
        [MaxLength(100)]
        public int ProjectId { get; set; } // "draft" or "pending"

        [Required]
        [MaxLength(100)]
        public string ContractCode { get; set; }

        [Required]
        public int FreelancerId { get; set; }

        [Required]
        [MaxLength(255)]
        public string FreelancerName { get; set; }

        [Required]
        [MaxLength(255)]
        public string OperatorName { get; set; }

        [Required]
        public int OperatorProfileId { get; set; }

        [Required]
        public DateTime ContractStartDate { get; set; }

        [Required]
        public DateTime ContractEndDate { get; set; }

        [Required]
        public DateTime ContractSignedDate { get; set; }

        [MaxLength(255)]
        public string ContractSignedOperatorByName { get; set; }

        [MaxLength(255)]
        public string ContractSignedFreelancerByName { get; set; }

       
    }
}