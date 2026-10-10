using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobseekerProfile
{
    [Table("JobSeekerExperienceAttachments")]
    public class JobSeekerExperienceAttachment
    {
        public int Id { get; set; }

        public int JobSeekerExperienceId { get; set; }

        public JobSeekerExperience JobSeekerExperience { get; set; }

        public int AttachmentId { get; set; }

        public Attachment Attachment { get; set; }

        [Required]
        [MaxLength(32)]
        public string Purpose { get; set; }

        public int SortOrder { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
