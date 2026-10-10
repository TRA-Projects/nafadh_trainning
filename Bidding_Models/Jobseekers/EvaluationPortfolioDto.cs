using System.Collections.Generic;

namespace Bidding_Models.Jobseekers
{
    public class JobSeekerEvaluationPortfolioListRowDto
    {
        public int profileId { get; set; }
        public string jobSeekerCode { get; set; }
        public string nameEn { get; set; }
        public string nameAr { get; set; }
        /// <summary>CURRENT status: <c>employed</c>, <c>trainee</c>, <c>hiring</c> or <c>jobseeker</c>.</summary>
        public string employmentStatus { get; set; }
        public int nominationsCount { get; set; }
        public int interviewsCount { get; set; }
        public int evaluationsCount { get; set; }
        public double? overallScoreOutOf100 { get; set; }
        public string specializationEn { get; set; }
        public string specializationAr { get; set; }
        public string wilayatEn { get; set; }
        public string wilayatAr { get; set; }
        /// <summary>Qualifying recommendation notes for this list, after the same source/date filters.</summary>
        public int recommendationCount { get; set; }
        /// <summary>ISO-8601 completion time of the latest qualifying recommendation note.</summary>
        public string latestRecommendationDateIso { get; set; }
    }

    public class JobSeekerEvaluationPortfolioSkillRatingDto
    {
        public string skillId { get; set; }
        public string nameEn { get; set; }
        public string nameAr { get; set; }
        public double? ratingOutOf5 { get; set; }
        public double? scoreOutOf100 { get; set; }
        public int assessmentCount { get; set; }
    }

    public class JobSeekerEvaluationPortfolioRecommendationDto
    {
        public string text { get; set; }
        public int count { get; set; }
        public List<string> sources { get; set; } = new List<string>();
        /// <summary>ISO-8601 completion time of the assessment that produced this note.</summary>
        public string dateIso { get; set; }
    }

    public class JobSeekerEvaluationPortfolioRejectionDto
    {
        public string reason { get; set; }
        public string interviewTitleEn { get; set; }
        public string interviewTitleAr { get; set; }
        public string interviewRecordId { get; set; }
        public string rejectedAtIso { get; set; }
    }

    public class JobSeekerEvaluationPortfolioActivityItemDto
    {
        public string id { get; set; }
        public string titleEn { get; set; }
        public string titleAr { get; set; }
        public string subtitleEn { get; set; }
        public string subtitleAr { get; set; }
        public string status { get; set; }
        public string dateIso { get; set; }

        /// <summary>Phase 3 audit: machine-readable event-type code (e.g. "application", "nomination",
        /// "interview", "assessment", "assessment_result", "employment", "withdrawal",
        /// "post_employment") — set only by Recent Activity; left empty/unused by every other consumer
        /// of this shared item shape (upcomingInterviews/applicationRequests/nextAssessments/
        /// latestNominations). Presentation label mapping stays entirely on the frontend, not baked
        /// into the backend as English/Arabic strings.</summary>
        public string eventType { get; set; }
    }

    /// <summary>One behavior from the BEHV catalog and how many submitted results recorded it.</summary>
    public class JobSeekerEvaluationPortfolioBehaviorReportDto
    {
        public string valueKey { get; set; }
        public string labelEn { get; set; }
        public string labelAr { get; set; }
        public int reportCount { get; set; }
    }

    public class JobSeekerEvaluationPortfolioFullResultDto
    {
        public string id { get; set; }
        public string sourceType { get; set; }
        public string titleEn { get; set; }
        public string titleAr { get; set; }
        public double? scoreOutOf100 { get; set; }
        public string status { get; set; }
        public string recommendations { get; set; }
        public string feedback { get; set; }
        public string completedAtIso { get; set; }

        /// <summary>
        /// Enrollment reference that opens the full result.
        /// Credential assessment: <c>CredentialEnrollment.Reference</c>.
        /// Interview: current seat for this profile and interview record.
        /// Null when the source has no enrollment.
        /// </summary>
        public string enrollmentReference { get; set; }
    }

    public class JobSeekerEvaluationPortfolioDetailDto : JobSeekerEvaluationPortfolioListRowDto
    {
        /// <summary>Phase 2: count of JobOpportunityParticipation rows with HasApplied=true.</summary>
        public int applicationsTotal { get; set; }

        /// <summary>Phase 2: applied rows where PipelineStatus is not "rejected" — the only terminal/closed
        /// value that exists anywhere in the current application pipeline (see JobsRepository.cs /
        /// job-applications-page.component.ts). Not a claim about hire/offer outcome, which lives in a
        /// separate table (EmploymentProcedureRecord) and is out of scope for this phase.</summary>
        public int applicationsActive { get; set; }

        /// <summary>Phase 2: applied rows where PipelineStatus == "rejected".</summary>
        public int applicationsClosed { get; set; }

        /// <summary>Phase 2: nominated rows (IsNominated=true) where PipelineStatus is not "rejected" —
        /// same "active" definition as applicationsActive, applied to the nomination flag instead.</summary>
        public int nominationsActive { get; set; }

        /// <summary>Phase 2: latest nominated rows (IsNominated=true), same shape/join pattern as
        /// applicationRequests below, ordered most-recent first.</summary>
        public List<JobSeekerEvaluationPortfolioActivityItemDto> latestNominations { get; set; } =
            new List<JobSeekerEvaluationPortfolioActivityItemDto>();

        /// <summary>Phase 2: InterviewEnrollment.Status bucketed into Scheduled/Pending/Confirmed/
        /// Rescheduled/Registered — the exact same set LoadUpcomingInterviewsAsync already uses.</summary>
        public int interviewsUpcoming { get; set; }

        /// <summary>Phase 2: InterviewEnrollment.Status == "Completed".</summary>
        public int interviewsCompleted { get; set; }

        /// <summary>Phase 2: everything else (Cancelled, No Show, Pending Submission, or any unrecognized
        /// value) — deliberately NOT labeled "Pending/Reschedule" since Rescheduled/Pending are already
        /// inside interviewsUpcoming per the existing bucket definition above.</summary>
        public int interviewsOther { get; set; }

        /// <summary>Phase 2: date of the nearest upcoming interview (first item of the already-loaded,
        /// date-ascending upcomingInterviews list), or null if none.</summary>
        public string nearestUpcomingInterviewDateIso { get; set; }

        /// <summary>Phase 3: true when at least one EmploymentProcedureRecord (Contracts table) exists
        /// for this profile. When false, all employment* fields below are null.</summary>
        public bool hasEmploymentProcedure { get; set; }

        /// <summary>Phase 3: raw EmploymentProcedureRecord.LifecycleStatus ("In Progress"/"Completed"/
        /// "Cancelled") of the selected current/most-relevant record — never a synthesized value.</summary>
        public string employmentLifecycleStatus { get; set; }

        /// <summary>Phase 3: raw EmploymentProcedureRecord.OfferResponseStatus.</summary>
        public string employmentOfferResponseStatus { get; set; }

        /// <summary>Phase 3: raw EmploymentProcedureRecord.ContractStatus.</summary>
        public string employmentContractStatus { get; set; }

        public string employmentOpportunityTitleEn { get; set; }
        public string employmentOpportunityTitleAr { get; set; }
        public string employmentCompanyNameEn { get; set; }
        public string employmentCompanyNameAr { get; set; }

        /// <summary>Phase 3: the most meaningful populated date on the selected record — preferring
        /// ContractRegisteredAt, then ContractStartDate, then OfferSentAt, then CreatedAt (always
        /// populated) — never a fabricated date.</summary>
        public string employmentRelevantDateIso { get; set; }

        /// <summary>Phase 3: raw PostEmploymentRecord.Status ("Active"/"Evaluation Pending"/"Terminated")
        /// for the most recently created PostEmploymentRecord on this profile, or null if none exists.</summary>
        public string postEmploymentStatus { get; set; }

        /// <summary>Phase 3: PostEmploymentRecord.UpdatedAt for the selected record (reflects the most
        /// recent status change), or null if none exists.</summary>
        public string postEmploymentDateIso { get; set; }

        /// <summary>Phase 3: raw WithdrawalRequest.Status of the most recently submitted request on this
        /// profile (any outcome — Submitted/UnderReview/InformationRequested/Approved/Rejected/Withdrawn),
        /// or null if none exists.</summary>
        public string withdrawalStatus { get; set; }

        /// <summary>Phase 3: raw WithdrawalRequest.RequestType ("Withdrawal"/"EarlyTermination").</summary>
        public string withdrawalRequestType { get; set; }

        /// <summary>Phase 3: WithdrawalRequest.SubmittedAt for the selected request, or null if none.</summary>
        public string withdrawalSubmittedAtIso { get; set; }

        /// <summary>Phase 3 audit: derived boolean — true when withdrawalStatus is one of the platform's
        /// existing "still open" statuses. Uses `WithdrawalRequestRepository.ActiveStatuses` directly
        /// (the same authoritative rule the Withdrawal Requests module itself uses), so this "is a
        /// withdrawal still open" definition exists in exactly one place, never duplicated on the
        /// frontend.</summary>
        public bool hasActiveWithdrawal { get; set; }

        /// <summary>Phase 3: latest ~10 meaningful events composed from data already loaded elsewhere in
        /// this DTO (applications, nominations, interviews, assessments, employment, withdrawal,
        /// post-employment) — not a new audit-log source, purely a re-presentation of existing fields,
        /// newest first.</summary>
        public List<JobSeekerEvaluationPortfolioActivityItemDto> recentActivity { get; set; } =
            new List<JobSeekerEvaluationPortfolioActivityItemDto>();

        public List<JobSeekerEvaluationPortfolioSkillRatingDto> professionalSkills { get; set; } =
            new List<JobSeekerEvaluationPortfolioSkillRatingDto>();

        public List<JobSeekerEvaluationPortfolioSkillRatingDto> basicSkills { get; set; } =
            new List<JobSeekerEvaluationPortfolioSkillRatingDto>();

        public List<JobSeekerEvaluationPortfolioSkillRatingDto> techStacks { get; set; } =
            new List<JobSeekerEvaluationPortfolioSkillRatingDto>();

        public List<JobSeekerEvaluationPortfolioRecommendationDto> topRecommendations { get; set; } =
            new List<JobSeekerEvaluationPortfolioRecommendationDto>();

        /// <summary>Assessment / interview feedback remarks (separate from recommendations).</summary>
        public List<JobSeekerEvaluationPortfolioRecommendationDto> remarks { get; set; } =
            new List<JobSeekerEvaluationPortfolioRecommendationDto>();

        public List<JobSeekerEvaluationPortfolioRejectionDto> rejectionReasons { get; set; } =
            new List<JobSeekerEvaluationPortfolioRejectionDto>();

        public List<JobSeekerEvaluationPortfolioActivityItemDto> upcomingInterviews { get; set; } =
            new List<JobSeekerEvaluationPortfolioActivityItemDto>();

        public List<JobSeekerEvaluationPortfolioActivityItemDto> applicationRequests { get; set; } =
            new List<JobSeekerEvaluationPortfolioActivityItemDto>();

        public List<JobSeekerEvaluationPortfolioActivityItemDto> nextAssessments { get; set; } =
            new List<JobSeekerEvaluationPortfolioActivityItemDto>();

        public List<JobSeekerEvaluationPortfolioFullResultDto> fullResults { get; set; } =
            new List<JobSeekerEvaluationPortfolioFullResultDto>();

        /// <summary>Top behaviors recorded on submitted assessment and interview results for this profile.</summary>
        public List<JobSeekerEvaluationPortfolioBehaviorReportDto> topBehaviors { get; set; } =
            new List<JobSeekerEvaluationPortfolioBehaviorReportDto>();
    }

    /// <summary>
    /// Jobseeker self-service recommendations page: evaluator notes + strongest / weakest skills.
    /// </summary>
    public class JobseekerMyRecommendationsDto
    {
        public List<JobSeekerEvaluationPortfolioRecommendationDto> recommendations { get; set; } =
            new List<JobSeekerEvaluationPortfolioRecommendationDto>();

        public List<JobSeekerEvaluationPortfolioSkillRatingDto> topSkills { get; set; } =
            new List<JobSeekerEvaluationPortfolioSkillRatingDto>();

        public List<JobSeekerEvaluationPortfolioSkillRatingDto> weakestSkills { get; set; } =
            new List<JobSeekerEvaluationPortfolioSkillRatingDto>();

        public double? overallScoreOutOf100 { get; set; }

        public double? latestResultScoreOutOf100 { get; set; }
    }

    /// <summary>
    /// Admin view of a selected jobseeker's recommendations, skill highlights, and completed reviews with scores.
    /// </summary>
    public class AdminJobseekerRecommendationsDto : JobseekerMyRecommendationsDto
    {
        public int profileId { get; set; }

        public string nameEn { get; set; }

        public string nameAr { get; set; }

        public string jobSeekerCode { get; set; }

        /// <summary>Completed interview / assessment / employer review events with scores.</summary>
        public List<JobSeekerEvaluationPortfolioFullResultDto> reviews { get; set; } =
            new List<JobSeekerEvaluationPortfolioFullResultDto>();
    }
}
