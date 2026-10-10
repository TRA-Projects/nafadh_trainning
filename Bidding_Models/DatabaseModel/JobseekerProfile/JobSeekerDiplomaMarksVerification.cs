using System;
using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel;

namespace Bidding_Models.DatabaseModel.JobseekerProfile
{
    [Table("JobSeekerDiplomaMarksVerifications")]
    public class JobSeekerDiplomaMarksVerification
    {
        public int Id { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; }

        public string Mathematics { get; set; }

        public string English { get; set; }

        public string Arabic { get; set; }

        public string CompleteGrade { get; set; }

        public string Status { get; set; }

        public DateTime SubmittedAt { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public int? ApprovedByProfileId { get; set; }
    }
}
