namespace Bidding_Models.DatabaseModel.JobOpportunities
{
    /// <summary>
    /// JobOpportunityParticipation.PipelineStatus values, exactly as persisted by JobsRepository
    /// (nominate → nominated, ask/apply transitions → pending_apply / pending_review, operator status
    /// updates → shortlisted / rejected). Mirrors the frontend JobApplicationPipelineStatus type. The
    /// jobseeker's ask-to-apply decline value lives in AskToApplyRejectionRules.DeclinedPipelineStatus.
    /// Stored values are compared case-insensitively by existing readers.
    /// </summary>
    public static class JobOpportunityPipelineStatuses
    {
        public const string Nominated = "nominated";
        public const string PendingApply = "pending_apply";
        public const string PendingReview = "pending_review";
        public const string Shortlisted = "shortlisted";
        public const string Rejected = "rejected";
    }
}
