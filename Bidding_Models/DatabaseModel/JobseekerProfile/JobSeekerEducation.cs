using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobseekerProfile
{
    [Table("JobSeekerEducations")]
    public class JobSeekerEducation
    {
        public int Id { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; }

        [MaxLength(512)]
        public string InstitutionText { get; set; }

        [MaxLength(16)]
        public string InstitutionConfigGroupCode { get; set; }

        [MaxLength(64)]
        public string InstitutionValueKey { get; set; }

        [Required]
        [MaxLength(64)]
        public string DegreeValueKey { get; set; }

        [MaxLength(256)]
        public string FieldOfStudyText { get; set; }

        [MaxLength(64)]
        public string FieldOfStudyValueKey { get; set; }

        [MaxLength(256)]
        public string SubSpecialty { get; set; }

        [Required]
        [MaxLength(8)]
        public string StartYear { get; set; }

        [Required]
        [MaxLength(8)]
        public string EndYear { get; set; }

        [MaxLength(32)]
        public string CumulativeGpa { get; set; }

        [MaxLength(128)]
        public string Grade { get; set; }

        public string Description { get; set; }

        /// <summary>True when the row was created from Nafath/MOL integration (locked filled fields).</summary>
        public bool FromIntegration { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<JobSeekerEducationAttachment> Attachments { get; set; } = new List<JobSeekerEducationAttachment>();
    }
}
