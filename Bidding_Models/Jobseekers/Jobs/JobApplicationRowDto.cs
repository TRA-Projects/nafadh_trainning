using System.Collections.Generic;

namespace Bidding_Models.Jobseekers.Jobs
{
    public class JobApplicationRowDto
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int ProfileId { get; set; }
        public string ApplicantName_en { get; set; }
        public string ApplicantName_ar { get; set; }
        public string StudentCode { get; set; }
        public string AppliedOn { get; set; }
        public string ApplicationType { get; set; }
        public int MatchPercent { get; set; }
        public string PipelineStatus { get; set; }
        public bool NominatedByTra { get; set; }
        /// <summary>Operator/admin asked this candidate to apply.</summary>
        public bool AskToApplySent { get; set; }
        /// <summary>Job seeker declined the ask-to-apply invitation.</summary>
        public bool AskToApplyRejected { get; set; }
        public string AskToApplyRejectionReason_en { get; set; }
        public string AskToApplyRejectionReason_ar { get; set; }
        /// <summary>Candidate submitted an application.</summary>
        public bool HasApplied { get; set; }
        public string University_en { get; set; }
        public string University_ar { get; set; }
        public List<ApplicationDocumentDto> UploadedDocuments { get; set; }
        public List<ApplicationAnswerDto> Answers { get; set; }
    }

    public class ApplicationDocumentDto
    {
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string FileName { get; set; }
        public long? FileSize { get; set; }
        public string FileType { get; set; }
        /// <summary>Stored attachment id for download via attachments API.</summary>
        public int? AttachmentId { get; set; }
        public string DataUrl { get; set; }
    }

    public class ApplicationAnswerDto
    {
        public string QuestionEn { get; set; }
        public string QuestionAr { get; set; }
        public string Answer { get; set; }
    }

    /// <summary>Job seeker's own submitted application row (My Applications list).</summary>
    public class JobSeekerMyApplicationRowDto
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string JobCode { get; set; }
        public string StudentCode { get; set; }
        public string JobTitleEn { get; set; }
        public string JobTitleAr { get; set; }
        public string CompanyEn { get; set; }
        public string CompanyAr { get; set; }
        public string AppliedOn { get; set; }
        public string Status { get; set; }
        public string ApplicationType { get; set; }
        public bool NominatedByTra { get; set; }
        public List<ApplicationDocumentDto> UploadedDocuments { get; set; }
        public List<ApplicationAnswerDto> Answers { get; set; }
    }

    /// <summary>Job seeker's inbox row: employer asked them to apply for an opportunity.</summary>
    public class JobSeekerAskToApplyRequestRowDto
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string JobCode { get; set; }
        public string JobTitleEn { get; set; }
        public string JobTitleAr { get; set; }
        public string CompanyEn { get; set; }
        public string CompanyAr { get; set; }
        public string AskedAt { get; set; }
        public string RequestStatus { get; set; }
        public string RejectionReason_en { get; set; }
        public string RejectionReason_ar { get; set; }
    }

    public class RejectAskToApplyRequestDto
    {
        public string ReasonValueKey { get; set; }
    }
}
