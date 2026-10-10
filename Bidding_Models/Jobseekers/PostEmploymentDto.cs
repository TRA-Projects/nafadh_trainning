using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bidding_Models.Jobseekers
{
    public class PostEmploymentSkillOptionDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("nameEn")]
        public string NameEn { get; set; }

        [JsonProperty("nameAr")]
        public string NameAr { get; set; }
    }

    public class TerminationReasonOptionDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("reasonEn")]
        public string ReasonEn { get; set; }

        [JsonProperty("reasonAr")]
        public string ReasonAr { get; set; }

        [JsonProperty("appliesToTypes")]
        public List<string> AppliesToTypes { get; set; }
    }

    public class EvaluationSkillRatingDto
    {
        [JsonProperty("skillId")]
        public string SkillId { get; set; }

        [JsonProperty("skillNameEn")]
        public string SkillNameEn { get; set; }

        [JsonProperty("skillNameAr")]
        public string SkillNameAr { get; set; }

        [JsonProperty("rating")]
        public decimal Rating { get; set; }
    }

    public class EngagementEvaluationDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("postEmploymentRecordId")]
        public string PostEmploymentRecordId { get; set; }

        [JsonProperty("contractId")]
        public string ContractId { get; set; }

        [JsonProperty("recordId")]
        public string RecordId { get; set; }

        [JsonProperty("candidateId")]
        public string CandidateId { get; set; }

        [JsonProperty("skillRatings")]
        public List<EvaluationSkillRatingDto> SkillRatings { get; set; }

        [JsonProperty("overallRating")]
        public decimal OverallRating { get; set; }

        [JsonProperty("outcome")]
        public string Outcome { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("notesEn")]
        public string NotesEn { get; set; }

        [JsonProperty("notesAr")]
        public string NotesAr { get; set; }

        [JsonProperty("evaluationDateIso")]
        public string EvaluationDateIso { get; set; }

        [JsonProperty("createdAtIso")]
        public string CreatedAtIso { get; set; }

        [JsonProperty("updatedAtIso")]
        public string UpdatedAtIso { get; set; }
    }

    public class TerminationRecordDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("postEmploymentRecordId")]
        public string PostEmploymentRecordId { get; set; }

        [JsonProperty("contractId")]
        public string ContractId { get; set; }

        [JsonProperty("recordId")]
        public string RecordId { get; set; }

        [JsonProperty("candidateId")]
        public string CandidateId { get; set; }

        [JsonProperty("terminationReasonCode")]
        public string TerminationReasonCode { get; set; }

        [JsonProperty("actualEndDateIso")]
        public string ActualEndDateIso { get; set; }

        [JsonProperty("isEarlyTermination")]
        public bool IsEarlyTermination { get; set; }

        [JsonProperty("terminationNotesEn")]
        public string TerminationNotesEn { get; set; }

        [JsonProperty("terminationNotesAr")]
        public string TerminationNotesAr { get; set; }

        [JsonProperty("initiatedBy")]
        public string InitiatedBy { get; set; }

        [JsonProperty("isVoided")]
        public bool IsVoided { get; set; }

        [JsonProperty("createdAtIso")]
        public string CreatedAtIso { get; set; }
    }

    public class PeriodicEvaluationSkillRatingDto
    {
        [JsonProperty("skillId")]
        public string SkillId { get; set; }

        [JsonProperty("rating")]
        public int Rating { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }
    }

    public class PeriodicEvaluationDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("postEmploymentRecordId")]
        public string PostEmploymentRecordId { get; set; }

        [JsonProperty("contractId")]
        public string ContractId { get; set; }

        [JsonProperty("candidateId")]
        public string CandidateId { get; set; }

        [JsonProperty("period")]
        public string Period { get; set; }

        [JsonProperty("evaluationMonthIndex")]
        public int EvaluationMonthIndex { get; set; }

        [JsonProperty("dueDateIso")]
        public string DueDateIso { get; set; }

        [JsonProperty("periodFromIso")]
        public string PeriodFromIso { get; set; }

        [JsonProperty("periodToIso")]
        public string PeriodToIso { get; set; }

        [JsonProperty("submittedAtIso")]
        public string SubmittedAtIso { get; set; }

        [JsonProperty("acknowledgedAtIso")]
        public string AcknowledgedAtIso { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("skillRatings")]
        public List<PeriodicEvaluationSkillRatingDto> SkillRatings { get; set; }

        [JsonProperty("overallRating")]
        public int? OverallRating { get; set; }

        [JsonProperty("strengths")]
        public string Strengths { get; set; }

        [JsonProperty("areasForImprovement")]
        public string AreasForImprovement { get; set; }

        [JsonProperty("overallFeedback")]
        public string OverallFeedback { get; set; }

        [JsonProperty("isDraft")]
        public bool IsDraft { get; set; }

        [JsonProperty("submittedByNote")]
        public string SubmittedByNote { get; set; }

        [JsonProperty("evaluatorReference")]
        public string EvaluatorReference { get; set; }

        [JsonProperty("createdAtIso")]
        public string CreatedAtIso { get; set; }

        [JsonProperty("updatedAtIso")]
        public string UpdatedAtIso { get; set; }
    }

    public class PostEmploymentRecordDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("employmentProcedureRecordId")]
        public string EmploymentProcedureRecordId { get; set; }

        [JsonProperty("contractId")]
        public string ContractId { get; set; }

        [JsonProperty("candidateId")]
        public string CandidateId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>Interview record reference when the contract came from Send Results.</summary>
        [JsonProperty("sourceInterviewRecordId")]
        public string SourceInterviewRecordId { get; set; }

        [JsonProperty("candidateNameEn")]
        public string CandidateNameEn { get; set; }

        [JsonProperty("candidateNameAr")]
        public string CandidateNameAr { get; set; }

        [JsonProperty("opportunityTitleEn")]
        public string OpportunityTitleEn { get; set; }

        [JsonProperty("opportunityTitleAr")]
        public string OpportunityTitleAr { get; set; }

        [JsonProperty("companyNameEn")]
        public string CompanyNameEn { get; set; }

        [JsonProperty("companyNameAr")]
        public string CompanyNameAr { get; set; }

        [JsonProperty("contractStartDateIso")]
        public string ContractStartDateIso { get; set; }

        [JsonProperty("evaluation")]
        public EngagementEvaluationDto Evaluation { get; set; }

        [JsonProperty("termination")]
        public TerminationRecordDto Termination { get; set; }

        [JsonProperty("periodicEvaluations")]
        public List<PeriodicEvaluationDto> PeriodicEvaluations { get; set; }

        [JsonProperty("createdAtIso")]
        public string CreatedAtIso { get; set; }

        [JsonProperty("updatedAtIso")]
        public string UpdatedAtIso { get; set; }
    }

    public class SaveEvaluationBodyDto
    {
        [JsonProperty("skillRatings")]
        public List<EvaluationSkillRatingDto> SkillRatings { get; set; }

        [JsonProperty("overallRating")]
        public decimal OverallRating { get; set; }

        [JsonProperty("notesEn")]
        public string NotesEn { get; set; }

        [JsonProperty("notesAr")]
        public string NotesAr { get; set; }
    }

    public class SaveTerminationBodyDto
    {
        [JsonProperty("terminationReasonCode")]
        public string TerminationReasonCode { get; set; }

        [JsonProperty("actualEndDateIso")]
        public string ActualEndDateIso { get; set; }

        [JsonProperty("isEarlyTermination")]
        public bool IsEarlyTermination { get; set; }

        [JsonProperty("initiatedBy")]
        public string InitiatedBy { get; set; }

        [JsonProperty("terminationNotesEn")]
        public string TerminationNotesEn { get; set; }

        [JsonProperty("terminationNotesAr")]
        public string TerminationNotesAr { get; set; }
    }

    public class CreatePeriodicEvaluationBodyDto
    {
        [JsonProperty("period")]
        public string Period { get; set; }

        [JsonProperty("periodFromIso")]
        public string PeriodFromIso { get; set; }

        [JsonProperty("periodToIso")]
        public string PeriodToIso { get; set; }

        [JsonProperty("evaluationMonthIndex")]
        public int? EvaluationMonthIndex { get; set; }

        [JsonProperty("dueDateIso")]
        public string DueDateIso { get; set; }

        [JsonProperty("skillRatings")]
        public List<PeriodicEvaluationSkillRatingDto> SkillRatings { get; set; }

        [JsonProperty("overallRating")]
        public int? OverallRating { get; set; }

        [JsonProperty("strengths")]
        public string Strengths { get; set; }

        [JsonProperty("areasForImprovement")]
        public string AreasForImprovement { get; set; }

        [JsonProperty("overallFeedback")]
        public string OverallFeedback { get; set; }

        [JsonProperty("submittedByNote")]
        public string SubmittedByNote { get; set; }

        [JsonProperty("evaluatorReference")]
        public string EvaluatorReference { get; set; }
    }
}
