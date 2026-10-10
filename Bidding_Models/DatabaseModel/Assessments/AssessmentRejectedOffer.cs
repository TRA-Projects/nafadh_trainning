using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel;
using Bidding_Models.DatabaseModel.Credentials;

namespace Bidding_Models.DatabaseModel.Assessments
{
    /// <summary>
    /// Data-entry record of an offer rejected by the job seeker, captured during assessment conduct.
    /// </summary>
    [Table("AssessmentRejectedOffers")]
    public class AssessmentRejectedOffer
    {
        public int Id { get; set; }

        public int CredentialEnrollmentId { get; set; }

        public CredentialEnrollment CredentialEnrollment { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; }

        /// <summary>Display text for the offering entity (denormalized from config or free text).</summary>
        [Required]
        [MaxLength(512)]
        public string EntityName { get; set; }

        /// <summary>System config value key under group <c>OFFR_ENTY</c>.</summary>
        [MaxLength(64)]
        public string EntityNameValueKey { get; set; }

        public DateTime OfferDate { get; set; }

        /// <summary>Optional offered salary as entered (free numeric/text).</summary>
        [MaxLength(64)]
        public string OfferedSalary { get; set; }

        /// <summary>System config value key under group <c>AOFF_RJCT</c>.</summary>
        [Required]
        [MaxLength(64)]
        public string RejectionReasonValueKey { get; set; }

        /// <summary>System config value key under group <c>AOFF_SUB</c> (optional linked sub).</summary>
        [MaxLength(64)]
        public string SubRejectionReasonValueKey { get; set; }

        public string Remarks { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
