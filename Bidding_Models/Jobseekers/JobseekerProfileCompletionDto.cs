using System;
using System.Collections.Generic;

namespace Bidding_Models.Jobseekers
{
    public class JobSeekerSidebarSummaryDto
    {
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        public string DateOfBirth { get; set; }
        public string Governorate { get; set; }
        public string Wilayat { get; set; }
        public string Address { get; set; }
        public string MaritalStatus { get; set; }
        public string HealthStatus { get; set; }
        public string SearcherStatus { get; set; }
        public string JobSearchDuration { get; set; }
        public string DateOfFirstRegistrationAsJobSeeker { get; set; }
        public string DateOfLastRegistrationAsJobSeeker { get; set; }
    }

    public class JobSeekerEducationSummaryDto
    {
        public string UniversityMajor { get; set; }
        public string EducationalInstitutions { get; set; }
        public string Grade { get; set; }
        public string EducationalLevel { get; set; }
        public string CountryOfGraduation { get; set; }
        public string YearOfGraduation { get; set; }
        public string Mathematics { get; set; }
        public string English { get; set; }
        public string Arabic { get; set; }
        public string CompleteGrade { get; set; }
    }

    public class ConfigLookupValueDto
    {
        public object id { get; set; }
        public string label_en { get; set; }
        public string label_ar { get; set; }
        public bool? subReasonDropdown { get; set; }
        public object parentReasonId { get; set; }
    }

    public class JobSeekerProfileAttachmentDto
    {
        public int attachmentId { get; set; }
        public string fileName { get; set; }
        public string fileType { get; set; }
        public int fileSize { get; set; }
        public string dataUrl { get; set; }
    }

    public class JobSeekerEducationRecordDto
    {
        public int Id { get; set; }
        public string Institution { get; set; }
        public ConfigLookupValueDto InstitutionLookup { get; set; }
        public string Degree { get; set; }
        public ConfigLookupValueDto FieldOfStudyLookup { get; set; }
        public string FieldOfStudy { get; set; }
        public string SubSpecialty { get; set; }
        public string StartYear { get; set; }
        public string EndYear { get; set; }
        public string CumulativeGpa { get; set; }
        public string Grade { get; set; }
        public string Description { get; set; }
        public JobSeekerProfileAttachmentDto Attachment { get; set; }
        /// <summary>Nafath/MOL draft or persisted integration row (filled fields locked).</summary>
        public bool FromIntegration { get; set; }
    }

    public class JobSeekerExperienceRowDto
    {
        public int Id { get; set; }
        public string Organization { get; set; }
        public string Role { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<string> SkillNames { get; set; } = new List<string>();
        public List<string> TechStack { get; set; } = new List<string>();
        public JobSeekerProfileAttachmentDto Attachment { get; set; }
        public bool FromIntegration { get; set; }
    }

    public class JobSeekerCertificateRowDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ConfigLookupValueDto NameLookup { get; set; }
        public string CertificateNumber { get; set; }
        public string Issuer { get; set; }
        public string AccreditingBodyWebsite { get; set; }
        public string ObtainedDate { get; set; }
        public JobSeekerProfileAttachmentDto Attachment { get; set; }
    }

    public class JobSeekerTrainingRowDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Provider { get; set; }
        public ConfigLookupValueDto ProviderLookup { get; set; }
        public string CompletedDate { get; set; }
        public string TrainingType { get; set; }
        public List<string> SkillNames { get; set; } = new List<string>();
        public List<string> TechStack { get; set; } = new List<string>();
        public JobSeekerProfileAttachmentDto CompletionAttachment { get; set; }
        public JobSeekerProfileAttachmentDto OtherAttachment { get; set; }
        public JobSeekerProfileAttachmentDto Attachment { get; set; }
        public bool FromIntegration { get; set; }
    }

    public class JobSeekerLanguageSkillDto
    {
        public string SkillName { get; set; }
        public string SkillLevel { get; set; }
        public bool FromIntegration { get; set; }
    }

    /// <summary>Wizard Next / Save: persist unsaved Nafath drafts for one or more sections.</summary>
    public class PersistNafathDraftsRequestDto
    {
        public List<JobSeekerEducationRecordDto> EducationRecords { get; set; }
        public List<JobSeekerExperienceRowDto> Experiences { get; set; }
        public List<JobSeekerTrainingRowDto> Trainings { get; set; }
        public List<JobSeekerLanguageSkillDto> Languages { get; set; }
    }

    public class PersistNafathDraftsResultDto
    {
        public List<JobSeekerEducationRecordDto> EducationRecords { get; set; } = new List<JobSeekerEducationRecordDto>();
        public List<JobSeekerExperienceRowDto> Experiences { get; set; } = new List<JobSeekerExperienceRowDto>();
        public List<JobSeekerTrainingRowDto> Trainings { get; set; } = new List<JobSeekerTrainingRowDto>();
    }

    public class JobSeekerPortfolioItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public List<string> SkillNames { get; set; } = new List<string>();
        public List<string> TechStack { get; set; } = new List<string>();
        public JobSeekerProfileAttachmentDto Attachment { get; set; }
    }

    public class AdminEducationRecordListItemDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string JobseekerName { get; set; }
        public string Institution { get; set; }
        public string Degree { get; set; }
        public string FieldOfStudy { get; set; }
        public string StartYear { get; set; }
        public string EndYear { get; set; }
    }

    public class AdminEducationRecordsPageDto
    {
        public List<AdminEducationRecordListItemDto> rows { get; set; } = new List<AdminEducationRecordListItemDto>();
        public int total { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }

    public class AdminEducationRecordDetailDto
    {
        public int ProfileId { get; set; }
        public string JobseekerName { get; set; }
        public string EmailAddress { get; set; }
        public JobSeekerEducationRecordDto Record { get; set; }
    }

    public class JobSeekerDiplomaMarksVerificationDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string Mathematics { get; set; }
        public string English { get; set; }
        public string Arabic { get; set; }
        public string CompleteGrade { get; set; }
        public string Status { get; set; }
        public DateTime SubmittedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }

    public class JobSeekerDiplomaMarksSaveDto
    {
        public string Mathematics { get; set; }
        public string English { get; set; }
        public string Arabic { get; set; }
        public string CompleteGrade { get; set; }
    }

    public class AdminDiplomaMarksVerificationListItemDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string JobseekerName { get; set; }
        public string Mathematics { get; set; }
        public string English { get; set; }
        public string Arabic { get; set; }
        public string CompleteGrade { get; set; }
        public string Status { get; set; }
        public DateTime SubmittedAt { get; set; }
        public JobSeekerProfileAttachmentDto Certificate { get; set; }
    }

    public class AdminDiplomaMarksVerificationsPageDto
    {
        public List<AdminDiplomaMarksVerificationListItemDto> rows { get; set; } =
            new List<AdminDiplomaMarksVerificationListItemDto>();
        public int total { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }

    public class AdminDiplomaMarksVerificationDetailDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string JobseekerName { get; set; }
        public string EmailAddress { get; set; }
        public string Mathematics { get; set; }
        public string English { get; set; }
        public string Arabic { get; set; }
        public string CompleteGrade { get; set; }
        public string Status { get; set; }
        public DateTime SubmittedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public JobSeekerProfileAttachmentDto Certificate { get; set; }
    }
}
