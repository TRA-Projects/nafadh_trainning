using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobseekerProfile
{
    [Table("JobSeekerPortfolioItems")]
    public class JobSeekerPortfolioItem
    {
        public int Id { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; }

        [Required]
        [MaxLength(512)]
        public string Title { get; set; }

        [MaxLength(2048)]
        public string Url { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<JobSeekerPortfolioItemAttachment> Attachments { get; set; } = new List<JobSeekerPortfolioItemAttachment>();

        public List<JobSeekerPortfolioItemSkill> Skills { get; set; } = new List<JobSeekerPortfolioItemSkill>();

        public List<JobSeekerPortfolioItemTechStack> TechStacks { get; set; } = new List<JobSeekerPortfolioItemTechStack>();
    }
}
