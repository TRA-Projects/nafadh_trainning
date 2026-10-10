using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel
{
    public class ProfileProject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProfileId { get; set; }
        public Profile Profile { get; set; }

        [Required, MaxLength(255)]
        public string ProjectTitle { get; set; }

        [Required, MaxLength(100)]
        public Enums.ProjectType Type { get; set; }  // Enum Instead of String

        [Required, MaxLength(100)]
        public int ProjectCategory { get; set; }

        [Required, MaxLength(50)]
        public string ProjectCode { get; set; }

        [Required, MaxLength(100)]
        public Enums.ExperienceLevel ExperienceLevel { get; set; }  // Enum Instead of String

        [Required, MaxLength(50)]
        public Enums.ProjectVisibility Visibility { get; set; } // Enum Instead of String

        [MaxLength(200)]
        public string PreferredFreelancerLocation { get; set; }

        [Required]
        public string ProjectDescription { get; set; }

        [Required]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime Deadline { get; set; }

        [Required, MaxLength(50)]
        public string ApplicationType { get; set; }
        public string Currency { get; set; }
        public DateTime? LastClarificationDate { get; set; }

        public int ApplicationCount { get; set; } = 0;

        [Required]
        public Enums.ProjectStatus Status { get; set; } 
        
        public BudgetTimeline BudgetTimeline { get; set; }

        // Collections for One-to-Many Relationships
        public ICollection<ProjectRequiredDocument> RequiredDocuments { get; set; } = new List<ProjectRequiredDocument>();
        public ICollection<ProjectQuestion> Questions { get; set; } = new List<ProjectQuestion>();
        public ICollection<ProjectAttachment> Attachments { get; set; } = new List<ProjectAttachment>();        
        public ICollection<UserSkills> RequiredSkills { get; set; } = new List<UserSkills>();

        public Enums.ProjectDurationEstimation DurationEstimation { get; set; } = 0;
        
        public ICollection<ProjectPhase> Phases { get; set; } = new List<ProjectPhase>();
        public Enums.ProjectVisibility ProjectVisibility { get; set; }

        //public ICollection<ProfileProjectContract> Contracts { get; set; } = new List<ProfileProjectContract>();

        [NotMapped]
        public ProfileProjectContract AwardedContract { get; set; } 
        public ICollection<ProfileProjectAudit> AuditLogs { get; set; }
            
    }

    public class BudgetTimeline
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }
        public ProfileProject Project { get; set; }

        [Required, MaxLength(50)]
        public string BudgetType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Budget { get; set; }

        [Required]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime Deadline { get; set; }
    }
    

    public class ProjectQuestion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }
        public ProfileProject Project { get; set; }

        [Required, MaxLength(50)]
        public string QuestionType { get; set; }

        [Required]
        public string Question { get; set; }

        [Required, MaxLength(20)]
        public string Importance { get; set; }
    }

    public class ProjectQuestionAnswer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }
        
        [Required]
        public int QuestionId { get; set; }
        
        [Required]
        public string Answer { get; set; }
        [Required]
        public bool IsCorrect { get; set; }
    }
    
    public class ProjectAttachment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }
        public ProfileProject Project { get; set; }

        [Required]
        public string AttachmentType { get; set; }

        [Required]
        public string AttachmentUrl { get; set; }

        [Required, MaxLength(255)]
        public string AttachmentName { get; set; }
    }
    public class ProjectRequiredDocument
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }        

        [Required, MaxLength(50)]
        public string Name { get; set; }
        public string FileType { get; set; }
    }

    public class ProfileProjectAudit
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int? ActionMadeByProfileId { get; set; }
        public DateTime ActionTime { get; set; }
        public string ActionTitle { get; set; }
        public string ActionTitleArabic { get; set; }
        //public Profile ActionMadeByProfile { get; set; }
    }
}
