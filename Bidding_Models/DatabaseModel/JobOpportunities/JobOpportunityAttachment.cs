using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.JobseekerProfile;

namespace Bidding_Models.DatabaseModel.JobOpportunities
{
    [Table("JobOpportunityAttachments")]
    public class JobOpportunityAttachment
    {
        public int Id { get; set; }

        public int JobOpportunityId { get; set; }

        public JobOpportunity JobOpportunity { get; set; }

        public int AttachmentId { get; set; }

        public Attachment Attachment { get; set; }

        [Required]
        [MaxLength(32)]
        public string Purpose { get; set; }

        [MaxLength(512)]
        public string DescriptionEn { get; set; }

        [MaxLength(512)]
        public string DescriptionAr { get; set; }

        public int SortOrder { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
