using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobseekerProfile
{
    [Table("ProfileAttachments")]
    public class ProfileAttachment
    {
        public int Id { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; }

        public int AttachmentId { get; set; }

        public Attachment Attachment { get; set; }

        [Required]
        [MaxLength(32)]
        public string Purpose { get; set; }

        public int SortOrder { get; set; }

        public bool IsPrimary { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
