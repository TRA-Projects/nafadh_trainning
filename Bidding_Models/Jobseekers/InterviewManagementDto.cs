using System;
using System.Collections.Generic;

namespace Bidding_Models.Jobseekers
{
    // -------------------------------------------------------------------------
    // Nested / shared shapes
    // -------------------------------------------------------------------------

    /// <summary>Per-candidate interview slot on an interview record.</summary>
    public class InterviewManagementCandidateScheduleDto
    {
        public string candidateId { get; set; }
        /// <summary>Job seeker profile id for this enrollment (stable across participation / IE-* candidate keys).</summary>
        public int? profileId { get; set; }
        /// <summary>Stable enrollment business key (e.g. IE-8001) from <see cref="DatabaseModel.Interviews.InterviewEnrollment.Reference"/>.</summary>
        public string enrollmentReference { get; set; }
        /// <summary>Seat attempt on this interview (1-based). Reschedule closes the current round and opens the next.</summary>
        public int roundNumber { get; set; } = 1;
        public string interviewDateIso { get; set; }
        public string interviewTime { get; set; }
        public int durationMinutes { get; set; }
        public string notes { get; set; }
        /// <summary>UI schedule slot state (Draft | Scheduled). Distinct from persisted enrollment pipeline status.</summary>
        public string status { get; set; }
        /// <summary>Persisted <see cref="DatabaseModel.Interviews.InterviewEnrollment.Status"/> for this candidate.</summary>
        public string enrollmentStatus { get; set; }
        /// <summary>True when a submitted (non-draft) JobPosting assessment exists for this enrollment.</summary>
        public bool hasSubmittedEvaluation { get; set; }
    }

    /// <summary>One skill rating row inside an interview evaluation.</summary>
    public class InterviewManagementSkillAssessmentRowDto
    {
        public string skillId { get; set; }
        /// <summary>
        /// <see cref="AssessmentSkillRatingKinds"/> value.
        /// Professional = opportunity skills; Basic = personal skills (catalog Kind=Personal).
        /// </summary>
        public string skillKind { get; set; }
        public int? rating { get; set; }
        public string notes { get; set; }
    }

    // -------------------------------------------------------------------------
    // Record responses (list, detail, mutation results)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Interview record returned from list, detail, and mutation endpoints.
    /// List and detail share this shape until DB migration introduces a slim list projection.
    /// </summary>
    public class InterviewManagementRecordDto
    {
        public string id { get; set; }
        public string titleEn { get; set; }
        public string titleAr { get; set; }
        public string opportunityId { get; set; }
        /// <summary>Denormalized from Job Posting; populated by server after integration.</summary>
        public string opportunityTitleEn { get; set; }
        /// <summary>Denormalized from Job Posting; populated by server after integration.</summary>
        public string opportunityTitleAr { get; set; }
        public string interviewType { get; set; }
        public string locationOrLink { get; set; }
        public string instructionsEn { get; set; }
        public string instructionsAr { get; set; }
        public List<string> interviewerIds { get; set; } = new List<string>();
        public List<string> skillIds { get; set; } = new List<string>();
        public List<string> additionalSkillIds { get; set; } = new List<string>();
        public List<string> associatedBasicSkillIds { get; set; } = new List<string>();
        public List<string> associatedTechStackIds { get; set; } = new List<string>();
        public string status { get; set; }
        public string notesEn { get; set; }
        public string notesAr { get; set; }
        public string interviewDateIso { get; set; }
        public List<string> registeredCandidateIds { get; set; } = new List<string>();
        public Dictionary<string, InterviewManagementCandidateScheduleDto> candidateSchedulesById { get; set; }
        public string conductOutcome { get; set; }
        public string conductReason { get; set; }
        public string rescheduledDateIso { get; set; }
        public string rescheduledTime { get; set; }
        public string assessmentState { get; set; }
        public string recruiterDecision { get; set; }
        public string recruiterDecisionAtIso { get; set; }
        public string recruiterDecisionReason { get; set; }
        public string reInterviewDateIso { get; set; }
        public string reInterviewTime { get; set; }
        public string createdById { get; set; }
        public string createdByNameEn { get; set; }
        public string createdByNameAr { get; set; }
        public string createdAtIso { get; set; }

        /// <summary>JobPosting | CredentialAssessment — distinguishes unified jobseeker Interviews inbox rows.</summary>
        public string sourceType { get; set; }

        /// <summary>Company name (JobPosting) or organization/source name such as TRA (CredentialAssessment).</summary>
        public string organizationNameEn { get; set; }

        /// <summary>Company name (JobPosting) or organization/source name such as TRA (CredentialAssessment).</summary>
        public string organizationNameAr { get; set; }

        /// <summary>Job/opportunity title (JobPosting) or assessment/exam/course title (CredentialAssessment).</summary>
        public string contextTitleEn { get; set; }

        /// <summary>Job/opportunity title (JobPosting) or assessment/exam/course title (CredentialAssessment).</summary>
        public string contextTitleAr { get; set; }

        /// <summary>Portal job id when <see cref="sourceType"/> is JobPosting.</summary>
        public int? relatedJobId { get; set; }

        /// <summary>Credential enrollment id when <see cref="sourceType"/> is CredentialAssessment.</summary>
        public string credentialEnrollmentId { get; set; }

        /// <summary>Credential definition id when <see cref="sourceType"/> is CredentialAssessment.</summary>
        public string credentialDefinitionId { get; set; }

        /// <summary>evaluation | exam | course when <see cref="sourceType"/> is CredentialAssessment.</summary>
        public string credentialKind { get; set; }
    }

    // -------------------------------------------------------------------------
    // Request bodies
    // -------------------------------------------------------------------------

    /// <summary>
    /// Request body for PUT records/{id}. Route supplies the record id.
    /// Server-owned audit fields and denormalized opportunity titles are preserved or resolved on the server.
    /// </summary>
    public class UpsertInterviewManagementRecordRequestDto
    {
        public string titleEn { get; set; }
        public string titleAr { get; set; }
        public string opportunityId { get; set; }
        public string interviewType { get; set; }
        public string locationOrLink { get; set; }
        public string instructionsEn { get; set; }
        public string instructionsAr { get; set; }
        public List<string> interviewerIds { get; set; } = new List<string>();
        public List<string> skillIds { get; set; } = new List<string>();
        public List<string> additionalSkillIds { get; set; } = new List<string>();
        public List<string> associatedBasicSkillIds { get; set; } = new List<string>();
        public List<string> associatedTechStackIds { get; set; } = new List<string>();
        public string status { get; set; }
        public string notesEn { get; set; }
        public string notesAr { get; set; }
        public string interviewDateIso { get; set; }
        /// <summary>
        /// Null = leave enrollments unchanged. Empty list clears seats.
        /// Must not default to [] — missing JSON would otherwise wipe seats on every upsert.
        /// </summary>
        public List<string> registeredCandidateIds { get; set; }
        public Dictionary<string, InterviewManagementCandidateScheduleDto> candidateSchedulesById { get; set; }
        public string conductOutcome { get; set; }
        public string conductReason { get; set; }
        public string rescheduledDateIso { get; set; }
        public string rescheduledTime { get; set; }
        public string assessmentState { get; set; }
        public string recruiterDecision { get; set; }
        public string recruiterDecisionAtIso { get; set; }
        public string recruiterDecisionReason { get; set; }
        public string reInterviewDateIso { get; set; }
        public string reInterviewTime { get; set; }

        /// <summary>Optional. Defaults to JobPosting for admin job-posting forms; CredentialAssessment is server-owned later.</summary>
        public string sourceType { get; set; }
    }

    /// <summary>Request body for PUT records/{id}/register.</summary>
    public class RegisterInterviewCandidatesRequestDto
    {
        public List<string> candidateIds { get; set; } = new List<string>();
        public string interviewDateIso { get; set; }
        public Dictionary<string, InterviewManagementCandidateScheduleDto> schedulesById { get; set; }
    }

    /// <summary>Request body for PUT records/{id}/recruiter-decision.</summary>
    public class RecruiterDecisionRequestDto
    {
        public string decision { get; set; }
        public string reason { get; set; }
        public string reInterviewDateIso { get; set; }
        public string reInterviewTime { get; set; }
    }

    /// <summary>
    /// Request body for PUT records/{recordId}/enrollments/{candidateId}/outcome.
    /// <see cref="reason"/> is accepted for API parity but not persisted (no enrollment reason column yet).
    /// </summary>
    public class InterviewEnrollmentOutcomeRequestDto
    {
        /// <summary>NoShow | Cancelled | Rescheduled</summary>
        public string outcome { get; set; }
        /// <summary>Optional operator note — not persisted until a schema column exists.</summary>
        public string reason { get; set; }
        /// <summary>Required when outcome is Rescheduled (yyyy-MM-dd).</summary>
        public string interviewDateIso { get; set; }
        /// <summary>Required when outcome is Rescheduled (HH:mm).</summary>
        public string interviewTime { get; set; }
    }

    // -------------------------------------------------------------------------
    // List / pool rows
    // -------------------------------------------------------------------------

    /// <summary>Flattened jobseeker interview row for the admin jobseeker interviews table (paged).</summary>
    public class InterviewManagementJobseekerListRowDto
    {
        public string trackId { get; set; }
        public string interviewRecordId { get; set; }
        public string candidateId { get; set; }
        /// <summary>Stable enrollment business key when this row represents a JobPosting interview enrollment.</summary>
        public string enrollmentReference { get; set; }
        public string opportunityId { get; set; }
        public string jobSeekerIdDisplay { get; set; }
        public string candidateNameEn { get; set; }
        public string candidateNameAr { get; set; }
        public string interviewTitleEn { get; set; }
        public string interviewTitleAr { get; set; }
        public string jobTitleEn { get; set; }
        public string jobTitleAr { get; set; }
        public string companyEn { get; set; }
        public string companyAr { get; set; }
        public string interviewDateIso { get; set; }
        public string interviewTime { get; set; }
        public string interviewType { get; set; }
        public string createdAtIso { get; set; }
        public string status { get; set; }
        /// <summary>Per-candidate enrollment pipeline status for this row.</summary>
        public string enrollmentStatus { get; set; }
        /// <summary>True when a submitted (non-draft) JobPosting assessment exists for this enrollment.</summary>
        public bool hasSubmittedEvaluation { get; set; }
        public string recruiterDecision { get; set; }

        /// <summary>JobPosting | CredentialAssessment.</summary>
        public string sourceType { get; set; }

        public string organizationNameEn { get; set; }
        public string organizationNameAr { get; set; }
        public string contextTitleEn { get; set; }
        public string contextTitleAr { get; set; }
        public int? relatedJobId { get; set; }
        public string credentialEnrollmentId { get; set; }
        public string credentialDefinitionId { get; set; }
        public string credentialKind { get; set; }
    }

    /// <summary>Candidate pool row for scheduling and evaluation flows.</summary>
    public class InterviewManagementCandidateDto
    {
        public string id { get; set; }
        public string nameEn { get; set; }
        public string nameAr { get; set; }
        public string email { get; set; }
        /// <summary>Mobile / phone for roster search (from profile).</summary>
        public string mobile { get; set; }
        public int? profileId { get; set; }
        public int? matchingPercent { get; set; }
        public string sourceTag { get; set; }
        public List<string> eligibleOpportunityIds { get; set; }
        /// <summary>Read-only pipeline status from JobOpportunityParticipations.</summary>
        public string pipelineStatus { get; set; }
        /// <summary>Read-only application date (ISO 8601).</summary>
        public string appliedOnIso { get; set; }
        /// <summary>Read-only student / staff code when available.</summary>
        public string studentCode { get; set; }
    }

    // -------------------------------------------------------------------------
    // Evaluations
    // -------------------------------------------------------------------------

    /// <summary>
    /// Interview evaluation/result for one candidate on one interview record.
    /// Used as PUT evaluations request body and response; keyed in GET evaluations map as "{interviewRecordId}::{candidateId}".
    /// </summary>
    public class InterviewManagementEvaluationDto
    {
        public string interviewRecordId { get; set; }
        public string candidateId { get; set; }
        /// <summary>Optional resolved profile id; server resolves from candidateId when not supplied.</summary>
        public int? profileId { get; set; }
        public List<InterviewManagementSkillAssessmentRowDto> skillAssessments { get; set; } = new List<InterviewManagementSkillAssessmentRowDto>();
        public int overallRating { get; set; }
        public string feedback { get; set; }
        public string recommendations { get; set; }
        public string finalDecision { get; set; }
        public string rejectionReason { get; set; }
        /// <summary>Optional system config value key for the rejection main reason.</summary>
        public string rejectionReasonValueKey { get; set; }
        /// <summary>Optional system config value key for the rejection sub-reason (child of main).</summary>
        public string subRejectionReasonValueKey { get; set; }
        public bool isDraft { get; set; }
        public string savedAtIso { get; set; }
        public string submittedAtIso { get; set; }

        /// <summary>System config value keys (group <c>BEHV</c>) observed during this interview.</summary>
        public List<string> behaviorValueKeys { get; set; }
        /// <summary>
        /// Populated on final submit so clients can refresh enrollment status without reloading all records.
        /// </summary>
        public InterviewManagementRecordDto updatedRecord { get; set; }
    }

    // -------------------------------------------------------------------------
    // Placeholder reference data (until Job Posting / user directory integration)
    // Not system-config lookups — fixed values/statuses come from Admin System config.
    // -------------------------------------------------------------------------

    /// <summary>
    /// Job opportunity option for interview create/edit forms.
    /// Placeholder until Job Posting module integration replaces GET opportunities.
    /// </summary>
    public class InterviewManagementOpportunityOptionDto
    {
        public string id { get; set; }
        public string titleEn { get; set; }
        public string titleAr { get; set; }
        public int jobId { get; set; }
        /// <summary>Job opportunity status (e.g. <c>UnderEvaluation</c>).</summary>
        public string status { get; set; }
        public List<string> defaultSkillIds { get; set; } = new List<string>();
        public List<string> defaultTechStackIds { get; set; } = new List<string>();
        /// <summary>Owner organization (English) from JobOpportunity.OwnerProfile when available.</summary>
        public string organizationNameEn { get; set; }
        /// <summary>Owner organization (Arabic) from JobOpportunity.OwnerProfile when available.</summary>
        public string organizationNameAr { get; set; }
    }

    /// <summary>
    /// Interviewer/user option for interviewer multi-select.
    /// Placeholder until user directory integration replaces GET interviewers.
    /// </summary>
    public class InterviewManagementUserOptionDto
    {
        public string id { get; set; }
        public string nameEn { get; set; }
        public string nameAr { get; set; }
    }

    // -------------------------------------------------------------------------
    // Legacy — not exposed by InterviewManagementController; frontend uses taxonomy API.
    // -------------------------------------------------------------------------

    [Obsolete("Use Admin System taxonomy API (getSkills). Not exposed by InterviewManagementController.")]
    public class InterviewManagementSkillOptionDto
    {
        public string id { get; set; }
        public string nameEn { get; set; }
        public string nameAr { get; set; }
    }

    [Obsolete("Use Admin System taxonomy API (getTechStacks). Not exposed by InterviewManagementController.")]
    public class InterviewManagementTechStackOptionDto
    {
        public string id { get; set; }
        public string nameEn { get; set; }
        public string nameAr { get; set; }
    }
}
