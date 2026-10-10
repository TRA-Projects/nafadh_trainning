using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Bidding_Models.DTOs
{
    public class ProjectApplicationRequestDto
    {
     

        public string Desigination { get; set; }

        public string CoverLetterUrl { get; set; }
        
        [Required(ErrorMessage = "EngagementType is required")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Enums.EngagementType EngagementType { get; set; }
        
        public decimal? BidAmount { get; set; }
        
        public string PricingSelection { get; set; }
        
        public int EstimationToCompleteTheProject { get; set; }
        
        public string CoverLetter { get; set; }
        
        public string WorkPlan { get; set; }
        
        public bool IsDigitalMarketplace { get; set; }
        
        public EvaluateAnswersDto EvaluateAnswers { get; set; }
        public List<EngagementAttachmentsDto> Attachments { get; set; } = new List<EngagementAttachmentsDto>();

    }

    public class ProjectApplicationResponseDto
    {
      
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int ProjectProfileId { get; set; }
        public int EngagementProfileId { get; set; }
        public string Status { get; set; }
        public string CoverLetterUrl { get; set; }
        public string ResumeUrl { get; set; }
        
        public string AdditionalFileUrl { get; set; }
        public string Designation { get; set; }
        public System.DateTime SubmissionDate { get; set; }
        public string Type { get; set; }
        public decimal? BidAmount { get; set; }
        
        public string PricingSelection { get; set; }
        
        public int EstimationToCompleteTheProject { get; set; }
        
        public string CoverLetter { get; set; }
        
        public string WorkPlan { get; set; }
        
        public bool IsDigitalMarketplace { get; set; }

        public List<EngagementAttachmentsDto> Attachments { get; set; } = new List<EngagementAttachmentsDto>();

    }
    public class ChangeStatusRequestDto
    {
        [Required]
        [EnumDataType(typeof(Enums.ProjectStatus))]
        public Enums.ProjectStatus Status { get; set; }
    }

    public class EngagementStatusRequestDto
    {
        [Required]
        [EnumDataType(typeof(Enums.EngagementStatus))]
        public Enums.EngagementStatus Status { get; set; }
        public string Remark { get; set; }
    }

    public class ApprovedProjectDto
    {
        public  int ProjectId { get; set; }
        public string ProjectName { get; set; }
    }

    public class EligibleFreelancerDto
    {
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileType { get; set; }
        public int EngagementId { get; set; }
    }
}