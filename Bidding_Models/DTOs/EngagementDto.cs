using Bidding_Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.DTOs
{
    public class EngagementResponseDto
    {
        

        public int Id { get; set; }

      
        public int EngagementProfileId { get; set; }

        
        public int ProjectId { get; set; }
        
        
        public int ProjectProfileId { get; set; }
        
        
        public string Type { get; set; }
        
       
        public DateTime SubmissionDate { get; set; }

       
        public string Status { get; set; }
        public string ChangeStatusRemark { get; set; }

        public string Designation { get; set; }
        public string ResumeUrl { get; set; }
        public string CoverLetterUrl { get; set; }
        public string AdditionalFileUrl { get; set; }
        
        public decimal? BidAmount { get; set; }
        //timeline 3 weeks  or 2 years 
        public object EngagementProfile { get; set; }
        
        public object EngagementSkills { get; set; }
        public object EngagementProfileWorkExperience { get; set; }
        public object EngagementProfileEducation { get; set; }
        public object EngagementProfileCertification { get; set; }
        public object EngagementProfilePortfolio { get; set; }
        public string WorkPlan { get; set; }
        public int EstimationToCompleteTheProject { get; set; }
        public string PricingSelection { get; set; }
        public List<EngagementAttachmentsDto> Attachments { get; set; }
        public string CoverLetter { get; set; }
        public List<EngagementAudit> AuditLogs { get; set; }
    }
    public class EngagementAttachmentsDto
    {
        [Required(ErrorMessage = "AttachmentType is required")]
        public string AttachmentType { get; set; }
        [Required(ErrorMessage = "AttachmentUrl is required")]
        public string AttachmentUrl { get; set; }
        [Required(ErrorMessage = "AttachmentName is required")]
        public string AttachmentName { get; set; }
        public int Id { get; set; }

        public int EngagementId { get; set; }
    }
}