using System.Collections.Generic;
using Bidding_Models.Jobseekers;
using Newtonsoft.Json;

namespace Bidding_Models.Jobseekers.Jobs
{
    public class JobPostJobDto
    {
        [JsonProperty("basic")]
        public JobPostBasicDraft Basic { get; set; }

        [JsonProperty("location")]
        public JobPostLocationDraft Location { get; set; }

        [JsonProperty("requirements")]
        public JobPostRequirementsDraft Requirements { get; set; }

        [JsonProperty("techStack")]
        public JobPostTechStackDraft TechStack { get; set; }

        [JsonProperty("compensation")]
        public JobPostCompensationDraft Compensation { get; set; }

        [JsonProperty("screening")]
        public JobPostScreeningDraft Screening { get; set; }

        [JsonProperty("attachments")]
        public List<AttachmentDraft> Attachments { get; set; }

        [JsonProperty("requiredDocuments")]
        public List<RequiredDocumentDraft> RequiredDocuments { get; set; }
    }

    public class JobPostBasicDraft
    {
        [JsonProperty("title_en")]
        public string Title_en { get; set; }

        [JsonProperty("title_ar")]
        public string Title_ar { get; set; }

        [JsonProperty("department_en")]
        public string Department_en { get; set; }

        [JsonProperty("department_ar")]
        public string Department_ar { get; set; }

        [JsonProperty("description_en")]
        public string Description_en { get; set; }

        [JsonProperty("description_ar")]
        public string Description_ar { get; set; }

        [JsonProperty("ownerProfileId")]
        public int OwnerProfileId { get; set; }

        [JsonProperty("jobReference")]
        public string JobReference { get; set; }

        [JsonProperty("categoryId")]
        public string CategoryId { get; set; }

        [JsonProperty("applicationTypes")]
        public List<string> ApplicationTypes { get; set; }

        [JsonProperty("openings")]
        public int Openings { get; set; }

        /// <summary>Cap on applications. Null/omitted = no cap.</summary>
        [JsonProperty("maxApplicants")]
        public int? MaxApplicants { get; set; }

        [JsonProperty("opportunityType")]
        public string JobType { get; set; }

        [JsonProperty("workMode")]
        public string WorkMode { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("isTra")]
        public bool IsTra { get; set; }

        [JsonProperty("traCode")]
        public string TraCode { get; set; }

        [JsonProperty("visibility")]
        public string Visibility { get; set; }

        [JsonProperty("urgency")]
        public string Urgency { get; set; }

        [JsonProperty("publicationDate")]
        public string PostedDate { get; set; }

        [JsonProperty("dueDate")]
        public string ExpiryDate { get; set; }

        /// <summary>System config <c>TGT_GEND</c> value key (e.g. both, male, female).</summary>
        [JsonProperty("targetGender")]
        public string TargetGender { get; set; }

        [JsonProperty("maleSeats")]
        public string MaleSeats { get; set; }

        [JsonProperty("femaleSeats")]
        public string FemaleSeats { get; set; }

        /// <summary>System config <c>INTV_MODE</c> value key.</summary>
        [JsonProperty("interviewMode")]
        public string InterviewMode { get; set; }
    }

    public class JobPostLocationDraft
    {
        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("officeAddress")]
        public string OfficeAddress { get; set; }

        [JsonProperty("timeZone")]
        public string TimeZone { get; set; }
    }

    public class JobPostRequirementsDraft
    {
        [JsonProperty("skills")]
        public List<int> Skills { get; set; }

        [JsonProperty("skillRequirements")]
        public List<SkillRequirementDraft> SkillRequirements { get; set; }

        [JsonProperty("secondarySkills")]
        public List<int> SecondarySkills { get; set; }

        [JsonProperty("yearsOfExperience")]
        public int YearsOfExperience { get; set; }

        [JsonProperty("yearsOfExperienceMargin")]
        public int YearsOfExperienceMargin { get; set; }

        [JsonProperty("educationLevel")]
        public string EducationLevel { get; set; }

        [JsonProperty("fieldOfStudy")]
        public List<string> FieldOfStudy { get; set; }

        [JsonProperty("languagesRequired")]
        public List<string> LanguagesRequired { get; set; }

        [JsonProperty("certificationsPreferred")]
        public List<string> CertificationsPreferred { get; set; }

        [JsonProperty("targetApplicant")]
        public string TargetApplicant { get; set; }
    }

    public class SkillRequirementDraft
    {
        [JsonProperty("skillId")]
        public int SkillId { get; set; }

        [JsonProperty("yearsOfExperience")]
        public int YearsOfExperience { get; set; }

        [JsonProperty("yearsMargin")]
        public int YearsMargin { get; set; }
    }

    public class JobPostTechStackDraft
    {
        [JsonProperty("techStackIds")]
        public List<int> TechStackIds { get; set; }

        [JsonProperty("techStackRequirements")]
        public List<TechStackRequirementDraft> TechStackRequirements { get; set; }
    }

    public class TechStackRequirementDraft
    {
        [JsonProperty("techStackId")]
        public int TechStackId { get; set; }

        [JsonProperty("yearsOfExperience")]
        public int YearsOfExperience { get; set; }

        [JsonProperty("yearsMargin")]
        public int YearsMargin { get; set; }
    }

    public class JobPostCompensationDraft
    {
        [JsonProperty("salaryDisplay")]
        public string SalaryDisplay { get; set; }

        [JsonProperty("salaryMin")]
        public decimal SalaryMin { get; set; }

        [JsonProperty("salaryMax")]
        public decimal SalaryMax { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("payFrequency")]
        public string PayFrequency { get; set; }

        [JsonProperty("benefits")]
        public List<string> Benefits { get; set; }

        [JsonProperty("startDate")]
        public string StartDate { get; set; }

        [JsonProperty("deadline")]
        public string Deadline { get; set; }

        [JsonProperty("budget")]
        public decimal Budget { get; set; }

        [JsonProperty("budgetType")]
        public string BudgetType { get; set; }

        [JsonProperty("projectEstimation")]
        public string ProjectEstimation { get; set; }
    }

    public class JobPostScreeningDraft
    {
        [JsonProperty("questions")]
        public List<OpportunityQuestionDraft> Questions { get; set; }

        [JsonProperty("screeningQuestion")]
        public string ScreeningQuestion { get; set; }

        [JsonProperty("screeningAnswerType")]
        public string ScreeningAnswerType { get; set; }
    }

    public class OpportunityQuestionDraft
    {
        public int? Id { get; set; }
        public string Question { get; set; }
        public string QuestionAr { get; set; }
        public string QuestionType { get; set; }
        public List<QuestionAnswerDraft> Answers { get; set; }
        public string Importance { get; set; }
    }

    public class QuestionAnswerDraft
    {
        public string Answer { get; set; }
        public string AnswerAr { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class AttachmentDraft
    {
        public int? LinkId { get; set; }
        public int? AttachmentId { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentType { get; set; }
        public string AttachmentUrl { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DataUrl { get; set; }
        public int? FileSize { get; set; }
    }

    public class RequiredDocumentDraft
    {
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string FileType { get; set; }
        public string Importance { get; set; }
    }

    public class JobOpportunityAttachmentUploadDto
    {
        public JobSeekerProfileAttachmentDto File { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public string AttachmentType { get; set; }
    }
}
