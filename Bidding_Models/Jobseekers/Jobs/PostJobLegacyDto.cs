using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bidding_Models.Jobseekers.Jobs
{
    /// <summary>
    /// Wire payload sent by the Angular job-posting wizard.
    /// Mirrors the TypeScript <c>PostJobDraftBody</c> interface built by <c>buildPostJobDraftBody()</c>.
    /// Adapted to <see cref="JobPostJobDto"/> by <c>PostJobLegacyAdapter</c> before reaching the service.
    /// </summary>
    public class PostJobLegacyDto
    {
        [JsonProperty("OwnerProfileId")]
        public int? OwnerProfileId { get; set; }

        [JsonProperty("JobReference")]
        public string JobReference { get; set; }

        /// <summary>"title_en | title_ar" — pipe-delimited.</summary>
        [JsonProperty("ProjectTitle")]
        public string ProjectTitle { get; set; }

        /// <summary>Integer DB id of the selected category (e.g. 6).</summary>
        [JsonProperty("ProjectCategory")]
        public int ProjectCategory { get; set; }

        /// <summary>"N years" display string — use <c>Clarification.YearsOfExperience</c> instead.</summary>
        [JsonProperty("ExperienceLevel")]
        public string ExperienceLevel { get; set; }

        /// <summary>salaryDisplay valueKey (e.g. "negotiable").</summary>
        [JsonProperty("ProjectVisibility")]
        public string ProjectVisibility { get; set; }

        /// <summary>opportunityType valueKey (e.g. "full-time").</summary>
        [JsonProperty("ProjectType")]
        public string ProjectType { get; set; }

        /// <summary>workMode valueKey (e.g. "hybrid").</summary>
        [JsonProperty("PreferredFreelancerLocation")]
        public string PreferredFreelancerLocation { get; set; }

        /// <summary>"&lt;p&gt;desc_en&lt;br/&gt;desc_ar&lt;/p&gt;" — HTML-wrapped combined description.</summary>
        [JsonProperty("ProjectDescription")]
        public string ProjectDescription { get; set; }

        /// <summary>JSON-encoded string containing all detail fields. Must be parsed separately.</summary>
        [JsonProperty("Clarification")]
        public string Clarification { get; set; }

        [JsonProperty("Currency")]
        public string Currency { get; set; }

        [JsonProperty("BudgetTimeline")]
        public PostJobLegacyBudgetTimeline BudgetTimeline { get; set; }

        [JsonProperty("Questions")]
        public List<PostJobLegacyQuestion> Questions { get; set; }

        [JsonProperty("Attachments")]
        public List<PostJobLegacyAttachment> Attachments { get; set; }

        [JsonProperty("Phases")]
        public List<object> Phases { get; set; }

        [JsonProperty("Deadline")]
        public string Deadline { get; set; }

        /// <summary>English labels: "TRA Nomination", "Candidate Apply", "Employer Search", "All".</summary>
        [JsonProperty("ApplicationType")]
        public List<string> ApplicationType { get; set; }

        [JsonProperty("ApplicationCount")]
        public int ApplicationCount { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("ProjectDurationEstimation")]
        public string ProjectDurationEstimation { get; set; }

        [JsonProperty("RequiredDocuments")]
        public List<PostJobLegacyRequiredDocument> RequiredDocuments { get; set; }

        /// <summary>Primary skill IDs.</summary>
        [JsonProperty("Skills")]
        public List<int> Skills { get; set; }
    }

    public class PostJobLegacyBudgetTimeline
    {
        [JsonProperty("Deadline")]
        public string Deadline { get; set; }

        [JsonProperty("StartDate")]
        public string StartDate { get; set; }

        /// <summary>Composite string ("1000 - 2000" or salary-display label). Not reliably parseable; use Clarification.SalaryMin/SalaryMax.</summary>
        [JsonProperty("Budget")]
        public string Budget { get; set; }

        /// <summary>payFrequency valueKey (e.g. "monthly").</summary>
        [JsonProperty("BudgetType")]
        public string BudgetType { get; set; }
    }

    public class PostJobLegacyQuestion
    {
        [JsonProperty("Id")]
        public int? Id { get; set; }

        [JsonProperty("Question")]
        public string Question { get; set; }

        [JsonProperty("QuestionAr")]
        public string QuestionAr { get; set; }

        [JsonProperty("QuestionType")]
        public string QuestionType { get; set; }

        [JsonProperty("Answers")]
        public List<PostJobLegacyQuestionAnswer> Answers { get; set; }

        [JsonProperty("Importance")]
        public string Importance { get; set; }
    }

    public class PostJobLegacyQuestionAnswer
    {
        [JsonProperty("Answer")]
        public string Answer { get; set; }

        [JsonProperty("AnswerAr")]
        public string AnswerAr { get; set; }

        [JsonProperty("IsCorrect")]
        public bool IsCorrect { get; set; }
    }

    public class PostJobLegacyAttachment
    {
        [JsonProperty("LinkId")]
        public int? LinkId { get; set; }

        [JsonProperty("AttachmentId")]
        public int? AttachmentId { get; set; }

        [JsonProperty("AttachmentName")]
        public string AttachmentName { get; set; }

        [JsonProperty("AttachmentType")]
        public string AttachmentType { get; set; }

        [JsonProperty("AttachmentUrl")]
        public string AttachmentUrl { get; set; }

        [JsonProperty("DescriptionEn")]
        public string DescriptionEn { get; set; }

        [JsonProperty("DescriptionAr")]
        public string DescriptionAr { get; set; }

        [JsonProperty("DataUrl")]
        public string DataUrl { get; set; }

        [JsonProperty("FileSize")]
        public int? FileSize { get; set; }
    }

    public class PostJobLegacyRequiredDocument
    {
        /// <summary>Same as NameEn — redundant field sent by Angular.</summary>
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("NameEn")]
        public string NameEn { get; set; }

        [JsonProperty("NameAr")]
        public string NameAr { get; set; }

        [JsonProperty("FileType")]
        public string FileType { get; set; }

        [JsonProperty("Importance")]
        public string Importance { get; set; }
    }
}
