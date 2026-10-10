using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Bidding_Models.DatabaseModel;
using Newtonsoft.Json.Converters;

namespace Bidding_Models.DTOs
{
    public class ProjectRequestDto
    {
        [Required(ErrorMessage = "Project Title is required ")]
        public string ProjectTitle { get; set; }
        
        [Required(ErrorMessage = "Project Type is required")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Enums.ProjectType ProjectType { get; set; }
        
        [Required(ErrorMessage = "Project Category is required most be number")]
        public int ProjectCategory { get; set; }               
        
        
        [Required(ErrorMessage = "ExperienceLevel is required")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Enums.ExperienceLevel ExperienceLevel { get; set; }
        [Required(ErrorMessage = "ProjectVisibility is required")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Enums.ProjectVisibility ProjectVisibility { get; set; }
        
        [Required(ErrorMessage = "PreferredFreelancerLocation is required")]
        public string PreferredFreelancerLocation { get; set; }
        public string Currency { get; set; }
        public DateTime Clarification { get; set; }
        
        [Required(ErrorMessage = "ProjectDescription")]
        public string ProjectDescription { get; set; }
        
        
        [Required(ErrorMessage = "BudgetTimeline is required")]
        public BudgetTimelineDto BudgetTimeline { get; set; }
     
        public DateTime Deadline { get; set; }
        
        
        [Required(ErrorMessage = "ApplicationType is required")]
        public string ApplicationType { get; set; }
        public int ApplicationCount { get; set; }
        [Required(ErrorMessage = "Status is required")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Enums.ProjectStatus Status { get; set; }
        
        public List<QuestionDto>Questions { get; set; }
        public List<AttachmentDto> Attachments { get; set; }
        public List<RequiredDocumentDto> RequiredDocuments { get; set; }
        
        public List<PhaseDto> Phases { get; set; }
        public Enums.ProjectDurationEstimation ProjectDurationEstimation { get; set; }
        public List<int> Skills { get; set; }
    }

    public class PhaseDto
    {
        public string PhaseName { get; set; }
        public string PhaseDescription { get; set; }
        public decimal Budget { get; set; }
        public int Id { get; set; }
        public int ProjectId { get; set; }
    }

    public class AttachmentDto
    {
        [Required(ErrorMessage = "AttachmentType is required")]
        public string AttachmentType { get; set; }
        [Required(ErrorMessage = "AttachmentUrl is required")]
        public string AttachmentUrl { get; set; }
        [Required(ErrorMessage = "AttachmentName is reqiored")]
        public string AttachmentName { get; set; }
        public int Id { get; set; }
        
        public int ProjectId { get; set; }
        
        public string Size { get; set; } 
    }

    public class RequiredDocumentDto
    {        
        public int? Id { get; set; }
        public int? ProjectId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "File type is required")]
        public string FileType { get; set; }
        public string Size { get; set; }
    }
    public class QuestionDto
    {
        [Required(ErrorMessage = "QuestionType is required")]
        public string QuestionType { get; set; }
        [Required(ErrorMessage = "Question is required")]
        public string Question { get; set; }
        [Required(ErrorMessage = "Importance is required")]
        public string Importance { get; set; }
        public int Id { get; set; }
        
        public AnswerDto[] Answers { get; set; }
    }

    public class AnswerDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Answer is required")]
        public string Answer { get; set; }
        [Required(ErrorMessage = "IsCorrect is required")]
        public bool IsCorrect { get; set; }
    
    }

    public class BudgetTimelineDto
    {
        public string BudgetType { get; set; }
        public decimal Budget { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime Deadline { get; set; }
        public int Id { get; set; }
        public int ProjectId { get; set; }
    }


    // list of projectResponseDto
    public class ProjectsResponseDto
        {
            public List<ProjectListDto> Projects { get; set; }
        }
    public class ProjectsResponseListDto
    {
        public List<ProjectListAllDto> Projects { get; set; }
    }


    public class ProjectListDto
        {
            public int Id { get; set; }
            
            public string ProjectTitle { get; set; }
            public string ProjectType { get; set; }
            public DateTime Deadline { get; set; }
            public string ApplicationType { get; set; }
            public int ApplicationCount { get; set; }
            public string Status { get; set; }
        }

    public class ProjectListAllDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string ProjectTitle { get; set; }
        public Enums.ProjectType ProjectType { get; set; }
        public DateTime Deadline { get; set; }
        public string ApplicationType { get; set; }
        public int ApplicationCount { get; set; }
        public string Status { get; set; }
        public BudgetTimelineDto BudgetTimeline { get; set; }
        
        public string ProjectProfileOwner { get; set; }
        
        public string ProjectProfileImage { get; set; }       

        public object ProjectCategory { get; set; }
        public bool Applied { get; set; }
        public bool IsFavorite { get; set; }
        public Enums.ExperienceLevel ExperienceLevel { get; set; }
        public string Currency { get; set; }
        public string PreferredFreelancerLocation { get; set; }
    }


    public class ProjectsResponseListMonitoringDto
    {
        public List<ProjectListMonitoringDto> Projects { get; set; }
    }



    public class ProjectListMonitoringDto
    {
        public int Id { get; set; }

        public string ProjectTitle { get; set; }
        
        public string ProjectCode { get; set; }

        public string FreeLancerName { get; set; }

        public string Client { get; set; }

        public int MilestoneProgress { get; set; }

        public string PaymentStatus { get; set; }                 

        public string Status { get; set; }
         
    }

    public class ProjectsResponseReportListDto
    {
        public List<ProjectReportListDto> Projects { get; set; }
    }


    public class ProjectReportListDto
    {
        public int Id { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectTitle { get; set; }
        public string ProjectType { get; set; }
        public object ProjectCategory { get; set; }
        public string ProjectVisibility { get; set; }
        public string ProjectApplicationType { get; set; }           
        public DateTime StartDate { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; }
        public string ApplicationCount { get; set; }
        public string OperatorName { get; set; }

    }


    public class ProjectResponseMonitoringDto
    {
        public int Id { get; set; }

        public string ProjectTitle { get; set; }

        public string ProjectCode { get; set; }

        public string FreeLancerName { get; set; }

        public string Client { get; set; }

        public int MilestoneProgress { get; set; }

        public string PaymentStatus { get; set; }

        public string Status { get; set; }

        public Enums.ProjectType ProjectType { get; set; }

        public string ApplicationType { get; set; }

        public DateTime Deadline { get; set; }

        public DateTime StartDate { get; set; }

        public int ProfileId { get; set; }

        public decimal Budget { get; set; }

        public string Currency { get; set; }

        public string BudgetType { get; set; }

        public List<AttachmentDto> Attachments { get; set; }
        public List<RequiredDocumentDto> RequiredDocuments { get; set; }

        public ProfileProjectContractDto Contract { get; set; }    
    }


    public class ProfileProjectContractDto
    {
        
        public int Id { get; set; }                                                
                        
        public string ContractType { get; set; } // bidding contract              
                        
        public string ContractStatus { get; set; } // draft or pending
                
        public string ContractCode { get; set; }
                
        public string FreelancerName { get; set; }
        public string DescriptionFileName { get; set; }

        public int FreelancerProfileId { get; set; }               
                
        public DateTime ContractStartDate { get; set; }
                
        public DateTime ContractEndDate { get; set; }

        public string ContractDescription { get; set; }

        public string PaymentTerm { get; set; } // 800 OMR                

        public decimal ContractAmount { get; set; }

        public ICollection<ProfileProjectMilestoneDto> Milestones { get; set; }
        
    }

    public class ProfileProjectMilestoneDto
    {        
        public int Id { get; set; }
                        
        public string MilestoneName { get; set; }
                
        public string MilestoneDescription { get; set; }
                
        public DateTime MilestoneDueDate { get; set; }

        public decimal? MilestoneAmount { get; set; }

        public decimal? MilestonePercentage { get; set; }

        public string MilestonePaymentType { get; set; } // fixed, hourly, percentage

        public string Deliverables { get; set; }

        public string DeliverableType { get; set; } // document, video

        public string MilestoneStatus { get; set; }

        public bool IsPaid { get; set; }

        public bool IsCompleted { get; set; }

        public string Remarks { get; set; }

        public int PhaseId { get; set; }

        public ICollection<ProfileProjectMilestoneOperationDto> MilestoneOperations { get; set; } = new List<ProfileProjectMilestoneOperationDto>();

    }

    public class ProfileProjectMilestoneOperationDto
    {
        public int Id { get; set; }                
        public string FreelancerComments { get; set; }
        public string OpratoreComments { get; set; }
        public string Reason { get; set; }
        public string Remark { get; set; }
        public string Description { get; set; }
        
        public ICollection<MilestoneAttachmentDto> Files { get; set; } = new List<MilestoneAttachmentDto>();
        public ICollection<MilestoneAttachmentDto> Links { get; set; } = new List<MilestoneAttachmentDto>();

    }

    public class MilestoneAttachmentDto
    {
        public int Id { get; set; }
        public string Attachment { get; set; }
        public Enums.AttachmentType Type { get; set; }
        public string Size { get; set; } // size in bytes
    }


    public class ProjectCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameAR { get; set; }
    }

    // single projectResponseDto
    public class ProjectResponseDto
    {
        public int Id { get; set; }

        public string Currency { get; set; }
        public DateTime? Clarification { get; set; }
        public int ApplicationCount { get; set; }
        public string ApplicationType { get; set; }
        public string Status { get; set; }
        public string ProjectTitle { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime StartDate { get; set; }

        public string ExperienceLevel { get; set; }

        public string ProjectVisibility { get; set; }
        public string PreferredFreelancerLocation { get; set; }
        public string ProjectCode { get; set; }


        public string ProjectProfileOwner { get; set; }
        public string ProjectProfileImage { get; set; }

        public string ProjectType { get; set; }
        public string ProjectDurationEstimation { get; set; }
        public string ProjectDescription { get; set; }
        public object ProjectCategory { get; set; }
        public BudgetTimelineDto BudgetTimeline { get; set; }
        public List<ProjectSkillsDto> Skills { get; set; }
        public List<RequiredDocumentDto> RequiredDocuments { get; set; }

        public List<QuestionDto> Questions { get; set; }
        public List<AttachmentDto> Attachments { get; set; }

        public List<PhaseDto> Phases { get; set; }
        public List<ProfileProjectAudit> AuditLogs { get; set; }
    }
    public class ProjectUpdateDto : ProjectRequestDto
        { 
           
           
        }

    public class ProjectSkillsDto
    {
        //id
        public int Id { get; set; }
        //name
        public string Name { get; set; }
        //isActive
        public bool IsActive { get; set; }
    }
    
    
    public class EvaluateAnswersDto
    {
        // list of question and answer like { questionId: 1, answerId: 1 }
        public List<AnswerFeedbackDto> Answers { get; set; }
        public int EngagementId { get; set; }
    }
    
    public class AnswerFeedbackDto
    {
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
       
    }
}