using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobseekerProfile
{
    [Table("JobSeekerCertificates")]
    public class JobSeekerCertificate
    {
        public int Id { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; }

        [Required]
        [MaxLength(512)]
        public string Name { get; set; }

        /// <summary>System config value key under group <c>CERT_NAME</c>.</summary>
        [MaxLength(64)]
        public string NameValueKey { get; set; }

        [Required]
        [MaxLength(128)]
        public string CertificateNumber { get; set; }

        [Required]
        [MaxLength(512)]
        public string Issuer { get; set; }

        [MaxLength(512)]
        public string AccreditingBodyWebsite { get; set; }

        public DateTime ObtainedDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<JobSeekerCertificateAttachment> Attachments { get; set; } = new List<JobSeekerCertificateAttachment>();
    }
}
