using System.Collections.Generic;

namespace Bidding_Models.Jobseekers.Jobs
{
    public class JobsFindRowDto
    {
        public int Id { get; set; }
        public bool? IsSaved { get; set; }
        /// <summary>True when the current job seeker has already applied to this opportunity.</summary>
        public bool? HasApplied { get; set; }
        public string Code { get; set; }
        public string JobReference { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string CompanyEn { get; set; }
        public string CompanyAr { get; set; }
        public string CategoryEn { get; set; }
        public string CategoryAr { get; set; }
        public string OpportunityType { get; set; }
        public string ApplicationType { get; set; }
        public string WorkMode { get; set; }
        public string DepartmentEn { get; set; }
        public string DepartmentAr { get; set; }
        public int Openings { get; set; }
        public string InterviewMode { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public int MatchPercent { get; set; }
        public List<string> SkillIds { get; set; }
        public List<string> SecondarySkillIds { get; set; }
        public List<string> TechStackIds { get; set; }
        public string LocationCountry { get; set; }
        public string LocationCity { get; set; }
        public string LocationOfficeAddress { get; set; }
        public string LocationTimeZone { get; set; }
        public int? YearsOfExperience { get; set; }
        public int? YearsOfExperienceMargin { get; set; }
        public string EducationLevel { get; set; }
        public string TargetApplicant { get; set; }
        public List<string> FieldOfStudy { get; set; }
        public List<string> LanguagesRequired { get; set; }
        public string CertificationsPreferred { get; set; }
        public string SalaryDisplay { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public string Currency { get; set; }
        public string PayFrequency { get; set; }
        public List<string> Benefits { get; set; }
        public string PostDate { get; set; }
        public string DueDate { get; set; }
        /// <summary>Count of job seekers who have applied (HasApplied).</summary>
        public int ApplicationsCount { get; set; }
        public string Status { get; set; }
        public List<JobsFindRequiredDocumentDto> RequiredDocuments { get; set; }
        public List<JobsFindAttachmentDto> Attachments { get; set; }
        public List<JobsFindScreeningQuestionDto> ScreeningQuestions { get; set; }
    }

    public class JobsFindRequiredDocumentDto
    {
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string FileType { get; set; }
        public string Importance { get; set; }
    }

    public class JobsFindAttachmentDto
    {
        public int? AttachmentId { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentType { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DataUrl { get; set; }
    }

    public class JobsFindScreeningQuestionDto
    {
        public int Id { get; set; }
        public string QuestionEn { get; set; }
        public string QuestionAr { get; set; }
        public string QuestionType { get; set; }
        public string Importance { get; set; }
        public List<JobsFindScreeningQuestionOptionDto> Options { get; set; }
    }

    public class JobsFindScreeningQuestionOptionDto
    {
        public int Id { get; set; }
        public string AnswerEn { get; set; }
        public string AnswerAr { get; set; }
    }
}
