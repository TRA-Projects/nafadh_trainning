using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobseekerProfile
{
    [Table("JobSeekerEducationSummaries")]
    public class JobSeekerEducationSummary
    {
        public int Id { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; }

        public string UniversityMajor { get; set; }

        public string EducationalInstitutions { get; set; }

        public string Grade { get; set; }

        public string EducationalLevel { get; set; }

        public string CountryOfGraduation { get; set; }

        public string YearOfGraduation { get; set; }

        public string Mathematics { get; set; }

        public string English { get; set; }

        public string Arabic { get; set; }

        public string CompleteGrade { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
