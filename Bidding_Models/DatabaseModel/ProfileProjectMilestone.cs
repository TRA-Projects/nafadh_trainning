using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace Bidding_Models.DatabaseModel
{
    public class ProfileProjectMilestone
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        [MaxLength(255)]
        public string MilestoneName { get; set; }

        [MaxLength(1000)]
        public string MilestoneDescription { get; set; }

        [Required]
        public DateTime MilestoneDueDate { get; set; }

        // Used when MilestonePaymentType is "fixed"
        public decimal? MilestoneAmount { get; set; }

        // Used when MilestonePaymentType is "percentage"
        public decimal? MilestonePercentage { get; set; }

        [Required]
        [MaxLength(50)]
        public string MilestonePaymentType { get; set; } // fixed, hourly, percentage

        [MaxLength(1000)]
        public string Deliverables { get; set; }

        [MaxLength(100)]
        public string DeliverableType { get; set; } // document, video

        [Required]
        [MaxLength(100)]
        public Enums.MilestoneStatus MilestoneStatus { get; set; }

        [Required]
        public bool IsPaid { get; set; }

        [Required]
        public bool IsCompleted { get; set; }

        // Audit fields
        [Required]
        [MaxLength(255)]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Foreign key to the main contract
        [Required]
        public int ContractId { get; set; }
        public ProfileProjectContract Contract { get; set; }
        public string Remarks { get; set; }
        
        [Required]
        public int PhaseId { get; set; }
        public ICollection<ProfileProjectMilestoneOperation> MilestoneOperations { get; set; } = new List<ProfileProjectMilestoneOperation>();

        public ICollection<Payment> Payment { get; set; } = new List<Payment>();
    }

    public class ProfileProjectMilestoneOperation
    {
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }

        [Required]
        public int MilestoneId { get; set; }
        public string FreelancerComments { get; set; }
        public string OpratoreCommint { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Authorization

        public ICollection<MilestoneAttachment> Attachments { get; set; } = new List<MilestoneAttachment>();
        public string Reason { get; set; }
        public string Remark { get; set; }
        public string Description { get; set; }
    }
    
    public class MilestoneAttachment
    {
        public int Id { get; set; }

        [Required]
        public int ProfileProjectMilestoneOperationsId { get; set; }
        public int ProfileProjectMilestoneOperationId { get; set; }

        public string Attachment { get; set; }
        public Enums.AttachmentType Type { get; set; }
    }
    public class DisputeResponseAttachment
    {
        public int Id { get; set; }

        [Required]
        public int DisputeId { get; set; }
        [Required]
        public string Attachment { get; set; }
    }
    public class Dispute
    {
        public int Id { get; set; }

        [Required]
        public int MilestoneId { get; set; }

        [Required]
        public int ContractId { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Authorization
        public string CreatedBy { get; set; }
        [Required]
        public int FreelancerId { get; set; }
        public Profile Freelancer { get; set; }
        [Required]
        public int OperatorProfileId { get; set; }

        public string Reason { get; set; }
        public string Status { get; set; }
        public string Resolution { get; set; }
        public string ResponseComment { get; set; }
        public string ResponseSuggestedResolution { get; set; }

        public ProfileProjectMilestone Milestone { get; set; }
        public ICollection<DisputeResponseAttachment> ReponseAttachments { get; set; }
        
    }
}