using System.Collections.Generic;

namespace Bidding_Models.Jobseekers
{
    public class PagedListResponseDto<T>
    {
        public List<T> rows { get; set; } = new List<T>();
        public int total { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }

    public class CredentialOkResponseDto
    {
        public bool ok { get; set; } = true;
    }

    public class CredentialDefinitionHubRowDto
    {
        public string id { get; set; }
        public string kind { get; set; }
        public string type { get; set; }
        public string provider { get; set; }
        public string titleEn { get; set; }
        public string titleAr { get; set; }
        public string descriptionEn { get; set; }
        public string descriptionAr { get; set; }
        public string instructionsEn { get; set; }
        public string instructionsAr { get; set; }
        public List<string> associatedSkillIds { get; set; } = new List<string>();
        public List<string> associatedBasicSkillIds { get; set; } = new List<string>();
        public List<string> associatedTechStackIds { get; set; } = new List<string>();
        public Dictionary<string, string> associatedSkillQuestions { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> associatedBasicSkillQuestions { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> associatedTechStackQuestions { get; set; } = new Dictionary<string, string>();
        public int weight { get; set; }
        public string status { get; set; }
        public string createdAtIso { get; set; }
    }

    public class CredentialHubSummaryDto
    {
        public int evaluations { get; set; }
        public int exams { get; set; }
        public int courses { get; set; }
        public int scheduledAssessments { get; set; }
    }

    public class CredentialLookupsDto
    {
        public List<SkillRefDto> skills { get; set; } = new List<SkillRefDto>();
        public List<SkillRefDto> basicSkills { get; set; } = new List<SkillRefDto>();
        public List<TechStackRefDto> techStacks { get; set; } = new List<TechStackRefDto>();
    }

    public class SkillRefDto
    {
        public string id { get; set; }
        public string nameEn { get; set; }
        public string nameAr { get; set; }
    }

    public class TechStackRefDto
    {
        public string id { get; set; }
        public string nameEn { get; set; }
        public string nameAr { get; set; }
    }

    public class JobSeekerLiteDto
    {
        public string id { get; set; }
        public string nameEn { get; set; }
        public string nameAr { get; set; }
        public string status { get; set; }
    }

    public class JobSeekerIdsValidateRequestDto
    {
        public List<string> ids { get; set; }
    }

    public class JobSeekerIdsValidateResponseDto
    {
        public List<string> invalidIds { get; set; } = new List<string>();
    }

    public class EvaluationDefinitionDto
    {
        public string id { get; set; }
        public string kind { get; set; } = "evaluation";
        public string titleEn { get; set; }
        public string titleAr { get; set; }
        public string descriptionEn { get; set; }
        public string descriptionAr { get; set; }
        public string instructionsEn { get; set; }
        public string instructionsAr { get; set; }
        public List<string> associatedSkillIds { get; set; } = new List<string>();
        public List<string> associatedBasicSkillIds { get; set; } = new List<string>();
        public List<string> associatedTechStackIds { get; set; } = new List<string>();
        public Dictionary<string, string> associatedSkillQuestions { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> associatedBasicSkillQuestions { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> associatedTechStackQuestions { get; set; } = new Dictionary<string, string>();
        public int weight { get; set; }
        public string status { get; set; }
        public string createdAtIso { get; set; }
    }

    public class ExamDefinitionDto
    {
        public string id { get; set; }
        public string kind { get; set; } = "exam";
        public string type { get; set; }
        public string titleEn { get; set; }
        public string titleAr { get; set; }
        public string descriptionEn { get; set; }
        public string descriptionAr { get; set; }
        public string instructionsEn { get; set; }
        public string instructionsAr { get; set; }
        public List<string> associatedSkillIds { get; set; } = new List<string>();
        public List<string> associatedBasicSkillIds { get; set; } = new List<string>();
        public List<string> associatedTechStackIds { get; set; } = new List<string>();
        public Dictionary<string, string> associatedSkillQuestions { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> associatedBasicSkillQuestions { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> associatedTechStackQuestions { get; set; } = new Dictionary<string, string>();
        public int weight { get; set; }
        public string status { get; set; }
        public string createdAtIso { get; set; }
    }

    public class CourseDefinitionDto
    {
        public string id { get; set; }
        public string kind { get; set; } = "course";
        public string provider { get; set; }
        public string titleEn { get; set; }
        public string titleAr { get; set; }
        public string descriptionEn { get; set; }
        public string descriptionAr { get; set; }
        public string instructionsEn { get; set; }
        public string instructionsAr { get; set; }
        public List<string> associatedSkillIds { get; set; } = new List<string>();
        public List<string> associatedBasicSkillIds { get; set; } = new List<string>();
        public List<string> associatedTechStackIds { get; set; } = new List<string>();
        public Dictionary<string, string> associatedSkillQuestions { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> associatedBasicSkillQuestions { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> associatedTechStackQuestions { get; set; } = new Dictionary<string, string>();
        public int weight { get; set; }
        public string status { get; set; }
        public string createdAtIso { get; set; }
    }

    public class EnrollmentRecordDto
    {
        public string id { get; set; }
        public string credentialId { get; set; }
        public string jobSeekerId { get; set; }
        public string registrationDateIso { get; set; }
        public string evaluationDateIso { get; set; }
        /// <summary>Scheduled duration in minutes. Null when not set.</summary>
        public int? durationMinutes { get; set; }
        public string status { get; set; }
        /// <summary>
        /// Client hint: jobseeker newly selected for this credential (no prior enrollment).
        /// Not persisted; server also detects missing previous rows.
        /// </summary>
        public bool? newlySelected { get; set; }
        /// <summary>
        /// Client hint: existing enrollment schedule/details changed (drives update notifications).
        /// Not persisted; server also detects date changes.
        /// </summary>
        public bool? scheduleUpdated { get; set; }
    }

    public class CreateEnrollmentsRequestDto
    {
        public List<EnrollmentRecordDto> rows { get; set; }
    }

    public class DeleteEnrollmentsRequestDto
    {
        public List<string> enrollmentIds { get; set; }
    }

    public class UpdateEnrollmentStatusRequestDto
    {
        public string status { get; set; }
    }

    /// <summary>
    /// Status-only update for a credential definition (evaluation / exam / course).
    /// Does not replace skill / basic-skill / tech-stack link rows.
    /// </summary>
    public class UpdateCredentialDefinitionStatusRequestDto
    {
        public string status { get; set; }
    }

    public class SkillRatingDto
    {
        public string skillId { get; set; }
        public int? numeric { get; set; }
        public string qualitative { get; set; }
    }

    [System.Obsolete("Use AssessmentBasicSkillRatings via AssessmentResultDto.skillRatings (CatalogBasicSkills).")]
    public class PersonalEvaluationDto
    {
        public int? commitment { get; set; }
        public int? communication { get; set; }
        public int? attendance { get; set; }
        public int? generalAppearance { get; set; }
        public int? confidence { get; set; }
        public string notes { get; set; }
    }

    public class CredentialResultDto
    {
        public string enrollmentId { get; set; }
        public List<SkillRatingDto> skillRatings { get; set; } = new List<SkillRatingDto>();
        public List<SkillRatingDto> techStackRatings { get; set; } = new List<SkillRatingDto>();
        public List<SkillRatingDto> basicSkillRatings { get; set; } = new List<SkillRatingDto>();
        public int? overallNumeric { get; set; }
        public string overallQualitative { get; set; }
        public double score { get; set; }
        public string dateTakenIso { get; set; }
        public string status { get; set; }
        public double accumulatedScore { get; set; }
        public string rescheduleEvaluationDateIso { get; set; }
        public PersonalEvaluationDto personalEvaluation { get; set; }

        // TODO: Persist recommendations after the DB migration adds the corresponding column.
        public string? recommendations { get; set; }

        /// <summary>System config value keys (group <c>BEHV</c>) observed during this assessment.</summary>
        public List<string> behaviorValueKeys { get; set; }
    }

    /// <summary>Rejected offer data-entry row captured during assessment conduct.</summary>
    public class AssessmentRejectedOfferDto
    {
        public int id { get; set; }
        public string enrollmentId { get; set; }
        public ConfigLookupValueDto entityNameLookup { get; set; }
        public string entityName { get; set; }
        public string offerDateIso { get; set; }
        public string offeredSalary { get; set; }
        public ConfigLookupValueDto rejectionReasonLookup { get; set; }
        public ConfigLookupValueDto subRejectionReasonLookup { get; set; }
        public string remarks { get; set; }
    }

    /// <summary>Jobseeker self-service list row — appointment metadata only (no results).</summary>
    public class JobseekerCredentialAppointmentListItemDto
    {
        public string enrollmentId { get; set; }
        public string credentialId { get; set; }
        public string kind { get; set; }
        public string titleEn { get; set; }
        public string titleAr { get; set; }
        public string evaluationDateIso { get; set; }
        public string status { get; set; }
    }

    /// <summary>Jobseeker self-service detail — appointment metadata only (no results).</summary>
    public class JobseekerCredentialAppointmentDetailDto : JobseekerCredentialAppointmentListItemDto
    {
        public string instructionsEn { get; set; }
        public string instructionsAr { get; set; }
        public string examType { get; set; }
        public string provider { get; set; }
        public List<SkillRefDto> professionalSkills { get; set; } = new List<SkillRefDto>();
        public List<SkillRefDto> basicSkills { get; set; } = new List<SkillRefDto>();
        public List<TechStackRefDto> techStacks { get; set; } = new List<TechStackRefDto>();
    }
}
