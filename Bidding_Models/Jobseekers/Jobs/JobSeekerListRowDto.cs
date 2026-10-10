using System.Collections.Generic;

namespace Bidding_Models.Jobseekers.Jobs
{
    /// <summary>Job id + bilingual title for job-seekers list nomination / ask-to-apply chips.</summary>
    public class JobSeekerListJobRefDto
    {
        public int JobId { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        /// <summary>Set on declined ask-to-apply chips.</summary>
        public string RejectionReason_en { get; set; }
        public string RejectionReason_ar { get; set; }
    }

    public class JobSeekerListRowDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string Name_en { get; set; }
        public string Name_ar { get; set; }
        public string JobSeekerCode { get; set; }
        public string Major_en { get; set; }
        public string Major_ar { get; set; }
        public string GraduationInstitute_en { get; set; }
        public string GraduationInstitute_ar { get; set; }
        public string GraduationDate { get; set; }
        public string EducationType { get; set; }
        public int MatchPercent { get; set; }
        public List<string> SkillIds { get; set; }
        public List<string> TechStackIds { get; set; }
        public List<int> RecommendedJobIds { get; set; }
        public List<int> NominatedJobIds { get; set; }
        /// <summary>Titles for NominatedJobIds (avoids client join to jobs list).</summary>
        public List<JobSeekerListJobRefDto> NominatedJobs { get; set; }
        public bool AskToApplySent { get; set; }
        public List<int> AskToApplyJobIds { get; set; }
        public List<JobSeekerListJobRefDto> AskToApplyJobs { get; set; }
        /// <summary>Ask-to-apply invitations this seeker declined, with the reason label.</summary>
        public List<JobSeekerListJobRefDto> AskToApplyDeclinedJobs { get; set; }
    }
}
