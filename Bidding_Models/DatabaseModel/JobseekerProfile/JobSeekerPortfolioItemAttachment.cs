using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobseekerProfile
{
    [Table("JobSeekerPortfolioItemAttachments")]
    public class JobSeekerPortfolioItemAttachment
    {
        public int Id { get; set; }

        public int JobSeekerPortfolioItemId { get; set; }

        public JobSeekerPortfolioItem JobSeekerPortfolioItem { get; set; }

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
