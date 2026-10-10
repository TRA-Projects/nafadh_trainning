using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobseekerProfile
{
    [Table("JobSeekerTrainings")]
    public class JobSeekerTraining
    {
        public int Id { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; }

        [Required]
        [MaxLength(512)]
        public string Title { get; set; }

        [Required]
        [MaxLength(512)]
        public string Provider { get; set; }

        /// <summary>System config value key under group <c>TRNG_PROV</c>.</summary>
        [MaxLength(64)]
        public string ProviderValueKey { get; set; }

        public DateTime? CompletedDate { get; set; }

        [Required]
        [MaxLength(64)]
        public string TrainingTypeValueKey { get; set; }

        /// <summary>True when the row was created from Nafath/MOL integration (locked filled fields).</summary>
        public bool FromIntegration { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<JobSeekerTrainingAttachment> Attachments { get; set; } = new List<JobSeekerTrainingAttachment>();

        public List<JobSeekerTrainingSkill> Skills { get; set; } = new List<JobSeekerTrainingSkill>();

        public List<JobSeekerTrainingTechStack> TechStacks { get; set; } = new List<JobSeekerTrainingTechStack>();
    }
}
