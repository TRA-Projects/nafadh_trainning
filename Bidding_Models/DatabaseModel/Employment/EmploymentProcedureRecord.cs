using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bidding_Models.DatabaseModel.Interviews;
using Bidding_Models.DatabaseModel.JobOpportunities;
using Bidding_Models.DatabaseModel.JobseekerProfile;

namespace Bidding_Models.DatabaseModel.Employment
{
    /// <summary>
    /// Single contracts row: offer phase → (on accept) contract phase → registration / post-employment.
    /// Mapped to table <c>Contracts</c>. Job/company labels via <see cref="JobOpportunityId"/> / <see cref="ProfileId"/>.
    /// </summary>
    [Table("Contracts")]
    public class EmploymentProcedureRecord
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string Reference { get; set; }

        public int ProfileId { get; set; }

        public Profile Profile { get; set; }

        public int JobOpportunityId { get; set; }

        public JobOpportunity JobOpportunity { get; set; }

        public int? InterviewRecordId { get; set; }

        public InterviewRecord InterviewRecord { get; set; }

        /// <summary><see cref="EmploymentContractPhases"/> — Offer (offers UI) or Contract (contracts UI).</summary>
        [Required]
        [MaxLength(32)]
        public string Phase { get; set; }

        [Required]
        [MaxLength(32)]
        public string LifecycleStatus { get; set; }

        [Required]
        [MaxLength(32)]
        public string OfferResponseStatus { get; set; }

        [Required]
        [MaxLength(32)]
        public string ContractStatus { get; set; }

        [MaxLength(32)]
        public string OfferCreationMode { get; set; }

        public int? ProbationPeriodDays { get; set; }

        public string AdditionalTermsEn { get; set; }

        public string AdditionalTermsAr { get; set; }

        public int? OfferAttachmentId { get; set; }

        public Attachment OfferAttachment { get; set; }

        public DateTime? OfferSentAt { get; set; }

        [MaxLength(64)]
        public string ContractType { get; set; }

        [MaxLength(8)]
        public string CurrencyCode { get; set; }

        [MaxLength(64)]
        public string PayFrequency { get; set; }

        public decimal? Amount { get; set; }

        /// <summary>When true, wage package is internal-only (not shown to the candidate).</summary>
        public bool IsPrivate { get; set; }

        public DateTime? IndicativeStartDate { get; set; }

        public DateTime? IndicativeEndDate { get; set; }

        public DateTime? ResponseAt { get; set; }

        [MaxLength(64)]
        public string RejectionReason { get; set; }

        [MaxLength(64)]
        public string SubRejectionReason { get; set; }

        public string ResponseNotesEn { get; set; }

        public string ResponseNotesAr { get; set; }

        [MaxLength(32)]
        public string AvailabilityCode { get; set; }

        public DateTime? AvailabilityDate { get; set; }

        public DateTime? ContractStartDate { get; set; }

        public DateTime? ContractEndDate { get; set; }

        public int? ContractAttachmentId { get; set; }

        public Attachment ContractAttachment { get; set; }

        public string ContractNotesEn { get; set; }

        public string ContractNotesAr { get; set; }

        public DateTime? ContractRegisteredAt { get; set; }

        public string CancelledReasonEn { get; set; }

        public string CancelledReasonAr { get; set; }

        public DateTime? ContractCancelledAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<ContractWageLine> WageLines { get; set; } = new List<ContractWageLine>();

        public List<ContractBenefitLine> BenefitLines { get; set; } = new List<ContractBenefitLine>();

        public List<EmploymentAcceptanceDocument> AcceptanceDocuments { get; set; } =
            new List<EmploymentAcceptanceDocument>();

        public List<EmploymentInquiryMessage> InquiryMessages { get; set; } =
            new List<EmploymentInquiryMessage>();

        public PostEmploymentRecord PostEmploymentRecord { get; set; }

        public List<WithdrawalRequest> WithdrawalRequests { get; set; } = new List<WithdrawalRequest>();
    }
}
