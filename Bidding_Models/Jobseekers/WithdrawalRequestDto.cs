using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bidding_Models.Jobseekers
{
    public class WithdrawalRequestAttachmentDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("mimeType")]
        public string MimeType { get; set; }

        [JsonProperty("sizeBytes")]
        public long SizeBytes { get; set; }

        [JsonProperty("uploadedAt")]
        public string UploadedAt { get; set; }
    }

    public class WithdrawalRequestOperatorResponseDto
    {
        [JsonProperty("decision")]
        public string Decision { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("rejectionReasonId")]
        public string RejectionReasonId { get; set; }

        [JsonProperty("informationQuestion")]
        public string InformationQuestion { get; set; }

        [JsonProperty("respondedAt")]
        public string RespondedAt { get; set; }

        [JsonProperty("respondedByOperatorId")]
        public string RespondedByOperatorId { get; set; }
    }

    public class SubmitWithdrawalRequestBodyDto
    {
        [JsonProperty("requestType")]
        public string RequestType { get; set; }

        [JsonProperty("employmentProcedureId")]
        public string EmploymentProcedureId { get; set; }

        [JsonProperty("contractId")]
        public string ContractId { get; set; }

        [JsonProperty("jobseekerId")]
        public string JobseekerId { get; set; }

        [JsonProperty("reasonCategory")]
        public string ReasonCategory { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("jobseekerNote")]
        public string JobseekerNote { get; set; }
    }

    public class UpdateWithdrawalRequestOperatorResponseBodyDto
    {
        [JsonProperty("decision")]
        public string Decision { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("rejectionReasonId")]
        public string RejectionReasonId { get; set; }

        [JsonProperty("informationQuestion")]
        public string InformationQuestion { get; set; }

        [JsonProperty("respondedByOperatorId")]
        public string RespondedByOperatorId { get; set; }
    }

    public class UpdateWithdrawalRequestStatusBodyDto
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("operatorResponse")]
        public UpdateWithdrawalRequestOperatorResponseBodyDto OperatorResponse { get; set; }
    }

    public class WithdrawalRequestDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("requestType")]
        public string RequestType { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("employmentProcedureId")]
        public string EmploymentProcedureId { get; set; }

        [JsonProperty("contractId")]
        public string ContractId { get; set; }

        [JsonProperty("postEmploymentId")]
        public string PostEmploymentId { get; set; }

        [JsonProperty("jobseekerId")]
        public string JobseekerId { get; set; }

        [JsonProperty("reasonCategory")]
        public string ReasonCategory { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("attachments")]
        public List<WithdrawalRequestAttachmentDto> Attachments { get; set; }

        [JsonProperty("jobseekerNote")]
        public string JobseekerNote { get; set; }

        [JsonProperty("operatorResponse")]
        public WithdrawalRequestOperatorResponseDto OperatorResponse { get; set; }

        [JsonProperty("submittedAt")]
        public string SubmittedAt { get; set; }

        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; set; }
    }
}
