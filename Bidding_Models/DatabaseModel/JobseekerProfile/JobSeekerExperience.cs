using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobseekerProfile
{
    [Table("JobSeekerExperiences")]
    public class JobSeekerExperience
    {
        public int Id { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; }

        [MaxLength(512)]
        public string OrganizationText { get; set; }

        [MaxLength(16)]
        public string OrganizationConfigGroupCode { get; set; }

        [MaxLength(64)]
        public string OrganizationValueKey { get; set; }

        [Required]
        [MaxLength(256)]
        public string Role { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        /// <summary>True when the row was created from Nafath/MOL integration (locked filled fields).</summary>
        public bool FromIntegration { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<JobSeekerExperienceAttachment> Attachments { get; set; } = new List<JobSeekerExperienceAttachment>();

        public List<JobSeekerExperienceSkill> Skills { get; set; } = new List<JobSeekerExperienceSkill>();

        public List<JobSeekerExperienceTechStack> TechStacks { get; set; } = new List<JobSeekerExperienceTechStack>();
    }
}
