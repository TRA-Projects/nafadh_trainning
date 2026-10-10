using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel;

namespace Bidding_Models.DatabaseModel.Credentials
{
    /// <summary>
    /// Links a job seeker profile to a credential definition (schedule + pipeline status).
    /// </summary>
    [Table("CredentialEnrollments")]
    public class CredentialEnrollment
    {
        public int Id { get; set; }

        /// <summary>
        /// Stable business / API identifier (e.g. EN-7001).
        /// Used in routes such as <c>PUT …/results/{enrollmentId}</c> and enrollment status updates —
        /// the frontend never sends the surrogate <see cref="Id"/>.
        /// </summary>
        [Required]
        [MaxLength(64)]
        public string Reference { get; set; }

        public int CredentialDefinitionId { get; set; }

        public CredentialDefinition CredentialDefinition { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; }

        public DateTime RegistrationDate { get; set; }

        public DateTime? EvaluationDate { get; set; }

        /// <summary>Scheduled duration in minutes (evaluation / exam / course seat).</summary>
        public int? DurationMinutes { get; set; }

        /// <summary>Selected, Pending, Scheduled, In Progress, Pending Submission, or Completed.</summary>
        [Required]
        [MaxLength(32)]
        public string Status { get; set; }
    }
}
