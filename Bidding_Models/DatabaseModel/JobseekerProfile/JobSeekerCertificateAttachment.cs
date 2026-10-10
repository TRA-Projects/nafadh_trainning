using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobseekerProfile
{
    [Table("JobSeekerCertificateAttachments")]
    public class JobSeekerCertificateAttachment
    {
        public int Id { get; set; }

        public int JobSeekerCertificateId { get; set; }

        public JobSeekerCertificate JobSeekerCertificate { get; set; }

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
