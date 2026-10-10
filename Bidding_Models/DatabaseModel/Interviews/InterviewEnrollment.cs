using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel;

namespace Bidding_Models.DatabaseModel.Interviews
{
    /// <summary>
    /// Links a job seeker profile to an interview record (schedule + pipeline status).
    /// Mirrors <see cref="Credentials.CredentialEnrollment"/>.
    /// </summary>
    [Table("InterviewEnrollments")]
    public class InterviewEnrollment
    {
        public int Id { get; set; }

        /// <summary>
        /// Stable business / API identifier (e.g. IE-8001).
        /// Used in routes and status updates — the frontend never sends the surrogate <see cref="Id"/>.
        /// </summary>
        [Required]
        [MaxLength(64)]
        public string Reference { get; set; }

        public int InterviewRecordId { get; set; }

        public InterviewRecord InterviewRecord { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; }

        /// <summary>
        /// Attempt number for this profile on the interview (1 = first seat).
        /// Unique with <see cref="InterviewRecordId"/> + <see cref="ProfileId"/> so reschedule
        /// can close round N and open round N+1 on the same interview.
        /// </summary>
        public int RoundNumber { get; set; } = 1;

        public DateTime RegistrationDate { get; set; }

        public DateTime? EvaluationDate { get; set; }

        /// <summary>Pending, In Progress, or Completed.</summary>
        [Required]
        [MaxLength(32)]
        public string Status { get; set; }
    }
}
