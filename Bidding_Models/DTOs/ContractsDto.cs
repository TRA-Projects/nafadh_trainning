using Bidding_Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Bidding_Models.Enums;

namespace Bidding_Models.DTOs
{
    public class ContractsOperatorsAllDto
    {
        public int ContractId { get; set; } // 5
        public string ProjectTitle { get; set; } // 
        public string ContractType { get; set; } // bidding contract
        public string FreelancerName { get; set; }
        public string PaymentTerm { get; set; } // 800 OMR
        public string ContractStartDate { get; set; }
        public string ContractEndDate { get; set; }
        public string ContractStatus { get; set; }
        public string ChangeStatusRemark { get; set; }
        public string ContractCode { get; set; }
        public int FreelancerProfileId { get; set; }
        public string OperatorName { get; set; }
        public int OperatorProfileId { get; set; }
        public decimal ContractAmount { get; set; }
        public string ProjectType { get; set; } // Hourly
        
        
        
        public int Rating { get; set; }
        
        // completion date
        public string CompletionDate { get; set; }
        public int ProjectId { get; set; }
    }
    public class ContractsOperatorsCompletedResponseDto
    {
        public int ContractId { get; set; } // 5
        public string ProjectTitle { get; set; } // 
        public string ContractType { get; set; } // bidding contract
        public string FreelancerName { get; set; }
        public string PaymentTerm { get; set; } // 800 OMR
        public string ContractStartDate { get; set; }
        public string ContractEndDate { get; set; }
        public string ContractStatus { get; set; }
        public string ChangeStatusRemark { get; set; }
        public string ContractCode { get; set; }
        public int FreelancerProfileId { get; set; }
        public string OperatorName { get; set; }
        public int OperatorProfileId { get; set; }
        public decimal ContractAmount { get; set; }
        public string ProjectType { get; set; } // Hourly
        public int Rating { get; set; }
        public string CompletionDate { get; set; }
        public int ProjectId { get; set; }
    }

    public class ContractsOperatorResponseDto
    {
        public int ContractId { get; set; }
        public string ContractType { get; set; }
        public string ContractStatus { get; set; }
        public string DescriptionFileName { get; set; }
        public string ChangeStatusRemark { get; set; }
        public string ContractCode { get; set; }
        public string ContractDescription { get; set; }
        public int FreelancerProfileId { get; set; }
        public string FreelancerName { get; set; }
       
        public int Rating { get; set; }
        public string FreelancerRole { get; set; } // developer or designer
        public string PreferredFreelancerLocation { get; set; } // Remote or Onsite
        public int OperatorProfileId { get; set; }
        public string OperatorName { get; set; }
        public string ContractEndDate { get; set; }
        
        public string ContractStartDate { get; set; }
       
        public string ProjectTitle { get; set; } 
        // software development or web digital marketing
    
        public string PaymentTerm { get; set; } // 800 OMR
        public string ProjectType { get; set; } // Hourly or Fixed
       
        
        public MilestoneDto[] Milestones { get; set; }
        public ContractTermsDto[] TermsAndConditions { get; set; } // terms and conditions
        public List<ContractAudit> AuditLogs { get; set; }
      
    }
    
    public class NewContractRequestDto
    {
  //     public int ContractId { get; set; }
       public int ProjectId { get; set; }
       public string ProjectTitle { get; set; }
       public string ContractType { get; set; } // bidding contract
       public string ProjectType { get; set; } // Hourly or Fixed
       public string ContractStatus { get; set; } //draft or pending
       public string ChangeStatusRemark { get; set; } 
       public string ContractCode { get; set; }
       public string FreelancerName { get; set; }
       public string DescriptionFileName { get; set; }
       public int FreelancerProfileId { get; set; }
       public string OperatorName { get; set; }
       public int OperatorProfileId { get; set; }
       public DateTime ContractStartDate { get; set; }
       public DateTime ContractEndDate { get; set; }
      
       public MilestoneDto[] Milestones { get; set; }
       public string ContractDescription { get; set; }
       public decimal ContractAmount { get; set; }
       public string PaymentTerm { get; set; }

    }
    
    public class MilestoneDto
    {
        public int ProjectId { get; set; }
        public int MilestoneId { get; set; }

        public string MilestoneName { get; set; }
        public string MilestoneDescription { get; set; }
        public DateTime MilestoneDueDate { get; set; }

        // Fixed amount in OMR (used when MilestonePaymentType is "fixed")
        public decimal? MilestoneAmount { get; set; }

        // Percentage value (e.g., 25.00 for 25%) when MilestonePaymentType is "percentage"
        public decimal? MilestonePercentage { get; set; }

        // "fixed", "hourly", or "percentage"
        public string MilestonePaymentType { get; set; }

        public string Deliverables { get; set; }
        public string DeliverableType { get; set; } // e.g., "document", "video"
        public Enums.MilestoneStatus MilestoneStatus { get; set; }

        public string StatusString { get; set; }

        public bool IsPaid { get; set; }
        public bool IsCompleted { get; set; }
        public string Remarks { get; set; }
        public IEnumerable<ProfileProjectMilestoneOperation> MilestoneOperations { get; set; } = new List<ProfileProjectMilestoneOperation>();
        public int PhaseId { get; set; }
    }
    public class ContractResponseDto
    {
        public int ContractId { get; set; }
        public string ProjectTitle { get; set; }
        public string ContractType { get; set; }
        public string DescriptionFileName { get; set; }
        public string ContractStatus { get; set; }
        public string ChangeStatusRemark { get; set; }
        public string ContractCode { get; set; }
        public string FreelancerName { get; set; }
        public int FreelancerProfileId { get; set; }
        public string OperatorName { get; set; }
        public int OperatorProfileId { get; set; }
        public decimal ContractAmount { get; set; }
        public string ContractStartDate { get; set; }
        public string ContractEndDate { get; set; }
        public string ProjectType { get; set; }
        public string PaymentTerm { get; set; }
        public string ContractDescription { get; set; }
        public List<ContractAudit> ContractAudit { get; set; }
    }

    public class SubmitRateDto
    {
        [Required]
        public int ProjectId { get; set; }
        [Required, Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        public string Testimonial { get; set; }

    }


    public class CompletedProjectDetailsResponseDto
    {
        public ProjectResponseDto ProjectDetails { get; set; }
        public MilestoneDto[] Milestones { get; set; }
        public AttachmentDto[] Attachments { get; set; }
        public string FreelancerName { get; set; }
        public int FreelancerProfileId { get; set; }
        public string ProjectStatus { get; set; }
        public string ProjectName { get; set; }
    }

    public class ContractsFreelancerAllDto
    {
        public int ContractId { get; set; } // 5
        public string ProjectTitle { get; set; } // 
        public string ContractType { get; set; } // bidding contract
        public string OperatorName { get; set; }
        public string PaymentTerm { get; set; } // 800 OMR
        public string ContractStartDate { get; set; }
        public string ContractEndDate { get; set; }
        public string ContractStatus { get; set; }
        public string ChangeStatusRemark { get; set; }
        public string ContractCode { get; set; }
        public int OperatorProfileId { get; set; }
        public decimal ContractAmount { get; set; }
        public string ProjectType { get; set; } // Hourly
        public bool IsContractSigned { get; set; }
        public int ProjectId { get; set; }
        public int OwnerProfileId { get; set; }
        public int EngagementId { get; set; }
        public ProjectStatus? ProjectStatus { get; set; }
    }

    public class ContractsFreelancerResponseDto
    {
        public int ContractId { get; set; }
        public int EngagementId { get; set; }
        public string OperatorName { get; set; }
        public string DescriptionFileName { get; set; }
        public string ProjectTitle { get; set; } 
        // software development or web digital marketing
        public string PaymentTerm { get; set; } // 800 OMR
        public string ProjectType { get; set; } // Hourly or Fixed
        public string ContractStartDate { get; set; }
        public string ContractDescription { get; set; }
        
        public string ContractType { get; set; }
        public string ContractStatus { get; set; }
        public string ChangeStatusRemark { get; set; }
        public string ContractCode { get; set; }
        public int OperatorProfileId { get; set; }
        public string ContractEndDate { get; set; }
        
        public bool IsContractSigned { get; set; }
        public MilestoneDto[] Milestones { get; set; }
        public ContractTermsDto[] TermsAndConditions { get; set; } // terms and conditions
        public string FreelancerName { get; set; }
        public List<ContractAudit> AuditLogs { get; set; }
    }

    public class SignContractDto
    {
        public int ContractId { get; set; }
      
        public string ContractCode { get; set; }
        public int FreelancerProfileId { get; set; }
        public string FreelancerName { get; set; }
        public string OperatorName { get; set; }
        public int OperatorProfileId { get; set; }
        public DateTime ContractStartDate { get; set; }
        public DateTime ContractEndDate { get; set; }
        public DateTime ContractSignedDate { get; set; }
        public string ContractSignedOperatorByName { get; set; } // operator or freelancer name
        public string ContractSignedFreelancerByName { get; set; } // operator or freelancer name
        public int ProjectId { get; set; }
    }
    public class ContractTermsDto
    {
        public string Title { get; set; }         
        public string Description { get; set; }  
    }

    public class ContractStatusRequestDto
    {
        [Required]
        [EnumDataType(typeof(Enums.ContractStatus))]
        public Enums.ContractStatus Status { get; set; }
        public string Remark { get; set; } = null;
    }

  
  
}