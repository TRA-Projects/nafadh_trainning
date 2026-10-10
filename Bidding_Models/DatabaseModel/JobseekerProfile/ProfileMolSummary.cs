using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobseekerProfile
{
    /// <summary>MOL-aligned job seeker profile snapshot (read-only source data for profile UI).</summary>
    [Table("ProfileMOLSummaries")]
    public class ProfileMolSummary
    {
        public int Id { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; }

        [MaxLength(256)]
        public string FullName { get; set; }

        [MaxLength(64)]
        public string Gender { get; set; }

        [MaxLength(16)]
        public string Age { get; set; }

        [MaxLength(32)]
        public string DateOfBirth { get; set; }

        [MaxLength(128)]
        public string Governorate { get; set; }

        [MaxLength(128)]
        public string Wilayat { get; set; }

        [MaxLength(64)]
        public string MaritalStatus { get; set; }

        [MaxLength(128)]
        public string HealthStatus { get; set; }

        [MaxLength(64)]
        public string SearcherStatus { get; set; }

        [MaxLength(64)]
        public string JobSearchDuration { get; set; }

        [MaxLength(32)]
        public string DateOfFirstRegistrationAsJobSeeker { get; set; }

        [MaxLength(32)]
        public string DateOfLastRegistrationAsJobSeeker { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
