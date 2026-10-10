using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel
{
    public class ProfileProjectContract
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        [MaxLength(255)]
        public string ProjectTitle { get; set; }


        [Required]
        [MaxLength(100)]
        public string ContractType { get; set; } // bidding contract

        [Required]
        [MaxLength(50)]
        public string ProjectType { get; set; } // Hourly or Fixed

        [Required]
        [MaxLength(100)]
        public string ContractStatus { get; set; } // draft or pending
        public string ChangeStatusRemark { get; set; } // draft or pending

        [Required]
        [MaxLength(100)]
        public string ContractCode { get; set; }

        [Required]
        [MaxLength(255)]
        public string FreelancerName { get; set; }
        public string DescriptionFileName { get; set; }

        [Required]
        public int FreelancerProfileId { get; set; }

        [Required]
        [MaxLength(255)]
        public string OperatorName { get; set; }

        [Required]
        public int OperatorProfileId { get; set; }

        [Required]
        public DateTime ContractStartDate { get; set; }

        [Required]
        public DateTime ContractEndDate { get; set; }


        public string ContractDescription { get; set; }

        [Required]
        public string PaymentTerm { get; set; } // 800 OMR

        // Audit fields
        [Required]
        [MaxLength(255)]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        public ProfileProject Project { get; set; }

        [NotMapped]
        public string ProjectStatus { get { return this.Project.Status.ToString(); }}

        // Navigation Property
        public ICollection<ProfileProjectMilestone> Milestones { get; set; }
        public decimal ContractAmount { get; set; }
        public ICollection<ContractAudit> AuditLogs { get; set; }

    }
    public class ContractAudit
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int? ActionMadeByProfileId { get; set; }
        public DateTime ActionTime { get; set; }
        public string ActionTitle { get; set; }
        public string ActionTitleArabic { get; set; }
        //public Profile ActionMadeByProfile { get; set; }
    }

}