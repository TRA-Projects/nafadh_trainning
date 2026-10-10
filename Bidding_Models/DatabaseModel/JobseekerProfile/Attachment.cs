using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobseekerProfile
{
    [Table("Attachments")]
    public class Attachment
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; }

        [Required]
        [MaxLength(128)]
        public string ContentType { get; set; }

        public long FileSizeBytes { get; set; }

        [Required]
        [MaxLength(512)]
        public string StorageKey { get; set; }

        [MaxLength(64)]
        public string ContentHash { get; set; }

        public int? UploadedByProfileId { get; set; }

        public Profile UploadedByProfile { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
