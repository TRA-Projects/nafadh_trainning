using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bidding_Models.Jobseekers
{
    public class ContractTypeOptionDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("nameEn")]
        public string NameEn { get; set; }

        [JsonProperty("nameAr")]
        public string NameAr { get; set; }
    }

    public class CurrencyOptionDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("nameEn")]
        public string NameEn { get; set; }

        [JsonProperty("nameAr")]
        public string NameAr { get; set; }

        [JsonProperty("isDefault")]
        public bool? IsDefault { get; set; }
    }

    public class PayFrequencyOptionDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("nameEn")]
        public string NameEn { get; set; }

        [JsonProperty("nameAr")]
        public string NameAr { get; set; }
    }

    public class RejectionReasonOptionDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("titleEn")]
        public string TitleEn { get; set; }

        [JsonProperty("titleAr")]
        public string TitleAr { get; set; }

        [JsonProperty("descriptionEn")]
        public string DescriptionEn { get; set; }

        [JsonProperty("descriptionAr")]
        public string DescriptionAr { get; set; }
    }

    public class InquiryMessageAttachmentDto
    {
        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("fileType")]
        public string FileType { get; set; }

        [JsonProperty("fileSizeBytes")]
        public long? FileSizeBytes { get; set; }

        [JsonProperty("fileUrl")]
        public string FileUrl { get; set; }

        [JsonProperty("attachmentId")]
        public int? AttachmentId { get; set; }
    }

    public class AcceptanceDocumentDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("titleEn")]
        public string TitleEn { get; set; }

        [JsonProperty("titleAr")]
        public string TitleAr { get; set; }

        [JsonProperty("descriptionEn")]
        public string DescriptionEn { get; set; }

        [JsonProperty("descriptionAr")]
        public string DescriptionAr { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("fileUrl")]
        public string FileUrl { get; set; }

        [JsonProperty("required")]
        public bool Required { get; set; }
    }

    public class CandidateResponseDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("recordId")]
        public string RecordId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("responseAtIso")]
        public string ResponseAtIso { get; set; }

        [JsonProperty("rejectionReasonId")]
        public string RejectionReasonId { get; set; }

        [JsonProperty("notesEn")]
        public string NotesEn { get; set; }

        [JsonProperty("notesAr")]
        public string NotesAr { get; set; }

        [JsonProperty("availabilityCode")]
        public string AvailabilityCode { get; set; }

        [JsonProperty("availabilityDateIso")]
        public string AvailabilityDateIso { get; set; }
    }

    public class ContractRecordDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("recordId")]
        public string RecordId { get; set; }

        [JsonProperty("contractTypeId")]
        public string ContractTypeId { get; set; }

        [JsonProperty("contractType")]
        public string ContractType { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("contractFile")]
        public InquiryMessageAttachmentDto ContractFile { get; set; }

        [JsonProperty("currencyCode")]
        public string CurrencyCode { get; set; }

        [JsonProperty("payFrequencyId")]
        public string PayFrequencyId { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("startDateIso")]
        public string StartDateIso { get; set; }

        [JsonProperty("endDateIso")]
        public string EndDateIso { get; set; }

        [JsonProperty("notesEn")]
        public string NotesEn { get; set; }

        [JsonProperty("notesAr")]
        public string NotesAr { get; set; }

        /// <summary>BENEF catalog value keys selected on contract registration.</summary>
        [JsonProperty("benefitIds")]
        public List<string> BenefitIds { get; set; }

        [JsonProperty("cancelledReasonEn")]
        public string CancelledReasonEn { get; set; }

        [JsonProperty("cancelledReasonAr")]
        public string CancelledReasonAr { get; set; }

        [JsonProperty("cancelledAt")]
        public string CancelledAt { get; set; }

        [JsonProperty("createdAtIso")]
        public string CreatedAtIso { get; set; }
    }

    public class InquiryMessageDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("recordId")]
        public string RecordId { get; set; }

        [JsonProperty("senderType")]
        public string SenderType { get; set; }

        [JsonProperty("messageEn")]
        public string MessageEn { get; set; }

        [JsonProperty("messageAr")]
        public string MessageAr { get; set; }

        [JsonProperty("createdAtIso")]
        public string CreatedAtIso { get; set; }

        [JsonProperty("attachments")]
        public List<InquiryMessageAttachmentDto> Attachments { get; set; }
    }

    public class OfferAllowanceLineDto
    {
        [JsonProperty("valueKey")]
        public string ValueKey { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }

    public class OfferBenefitLineDto
    {
        [JsonProperty("valueKey")]
        public string ValueKey { get; set; }

        [JsonProperty("remarksEn")]
        public string RemarksEn { get; set; }

        [JsonProperty("remarksAr")]
        public string RemarksAr { get; set; }

        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }

        [JsonProperty("fromOpportunity")]
        public bool FromOpportunity { get; set; }
    }

    public class ContractDraftDto
    {
        [JsonProperty("contractTypeId")]
        public string ContractTypeId { get; set; }

        [JsonProperty("contractType")]
        public string ContractType { get; set; }

        [JsonProperty("startDateIso")]
        public string StartDateIso { get; set; }

        [JsonProperty("endDateIso")]
        public string EndDateIso { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("currencyCode")]
        public string CurrencyCode { get; set; }

        [JsonProperty("payFrequencyId")]
        public string PayFrequencyId { get; set; }

        [JsonProperty("basicSalary")]
        public decimal? BasicSalary { get; set; }

        /// <summary>Wage-level private flag (entire package, not per allowance).</summary>
        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }

        [JsonProperty("allowances")]
        public List<OfferAllowanceLineDto> Allowances { get; set; }

        [JsonProperty("benefitLines")]
        public List<OfferBenefitLineDto> BenefitLines { get; set; }
    }

    public class OfferPreparationBodyDto
    {
        [JsonProperty("offerCreationMode")]
        public string OfferCreationMode { get; set; }

        [JsonProperty("probationPeriodDays")]
        public int? ProbationPeriodDays { get; set; }

        [JsonProperty("additionalTermsEn")]
        public string AdditionalTermsEn { get; set; }

        [JsonProperty("additionalTermsAr")]
        public string AdditionalTermsAr { get; set; }

        [JsonProperty("expectedStartDateIso")]
        public string ExpectedStartDateIso { get; set; }

        [JsonProperty("offerFile")]
        public InquiryMessageAttachmentDto OfferFile { get; set; }

        [JsonProperty("contractDraft")]
        public ContractDraftDto ContractDraft { get; set; }
    }

    public class EmploymentProcedureRecordDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("candidateId")]
        public string CandidateId { get; set; }

        [JsonProperty("candidateNameEn")]
        public string CandidateNameEn { get; set; }

        [JsonProperty("candidateNameAr")]
        public string CandidateNameAr { get; set; }

        [JsonProperty("companyNameEn")]
        public string CompanyNameEn { get; set; }

        [JsonProperty("companyNameAr")]
        public string CompanyNameAr { get; set; }

        [JsonProperty("opportunityId")]
        public string OpportunityId { get; set; }

        [JsonProperty("opportunityTitleEn")]
        public string OpportunityTitleEn { get; set; }

        [JsonProperty("opportunityTitleAr")]
        public string OpportunityTitleAr { get; set; }

        [JsonProperty("opportunityTypeEn")]
        public string OpportunityTypeEn { get; set; }

        [JsonProperty("opportunityTypeAr")]
        public string OpportunityTypeAr { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("acceptanceDocuments")]
        public List<AcceptanceDocumentDto> AcceptanceDocuments { get; set; }

        [JsonProperty("candidateResponse")]
        public CandidateResponseDto CandidateResponse { get; set; }

        [JsonProperty("contract")]
        public ContractRecordDto Contract { get; set; }

        [JsonProperty("sourceInterviewRecordId")]
        public string SourceInterviewRecordId { get; set; }

        [JsonProperty("offerCreationMode")]
        public string OfferCreationMode { get; set; }

        [JsonProperty("probationPeriodDays")]
        public int? ProbationPeriodDays { get; set; }

        [JsonProperty("offerFile")]
        public InquiryMessageAttachmentDto OfferFile { get; set; }

        [JsonProperty("additionalTermsEn")]
        public string AdditionalTermsEn { get; set; }

        [JsonProperty("additionalTermsAr")]
        public string AdditionalTermsAr { get; set; }

        [JsonProperty("expectedStartDateIso")]
        public string ExpectedStartDateIso { get; set; }

        [JsonProperty("basicSalary")]
        public decimal? BasicSalary { get; set; }

        /// <summary>Wage-level private flag (entire package, not per allowance).</summary>
        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }

        [JsonProperty("allowances")]
        public List<OfferAllowanceLineDto> Allowances { get; set; }

        [JsonProperty("benefitLines")]
        public List<OfferBenefitLineDto> BenefitLines { get; set; }

        /// <summary>BENEF keys configured on the related job opportunity (force-loaded into offer extras).</summary>
        [JsonProperty("opportunityBenefitValueKeys")]
        public List<string> OpportunityBenefitValueKeys { get; set; }

        [JsonProperty("socialInsuranceDeductionPercent")]
        public decimal? SocialInsuranceDeductionPercent { get; set; }

        [JsonProperty("createdAtIso")]
        public string CreatedAtIso { get; set; }

        [JsonProperty("updatedAtIso")]
        public string UpdatedAtIso { get; set; }

        /// <summary>When the offer was sent (`OfferSentAt`); null until sent.</summary>
        [JsonProperty("offerSentAtIso")]
        public string OfferSentAtIso { get; set; }

        /// <summary>When the contract was registered (`ContractRegisteredAt`); null until registered.</summary>
        [JsonProperty("contractRegisteredAtIso")]
        public string ContractRegisteredAtIso { get; set; }
    }
}
