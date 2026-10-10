namespace Bidding_Models.Jobseekers
{
    /// <summary>Stable programmatic keys for notification templates (seed-only; read-only in admin UI).</summary>
    public static class NotificationTemplateActionCodes
    {
        public const string AskToApply = "ASK_TO_APPLY";
        public const string JobApplied = "JOB_APPLIED";
        public const string ApplicationStatusUpdated = "APPLICATION_STATUS_UPDATED";
        public const string InterviewScheduled = "INTERVIEW_SCHEDULED";
        public const string OfferSubmitted = "OFFER_SUBMITTED";
        public const string OfferReissued = "OFFER_REISSUED";
        public const string OfferCandidateResponse = "OFFER_CANDIDATE_RESPONSE";
        public const string OfferInquiryReceived = "OFFER_INQUIRY_RECEIVED";
        public const string OpInterviewInvitation = "OP_INTERVIEW_INVITATION";
        public const string OpUpdInterview = "OP_UPD_INTERVIEW";
        public const string RegistrationEmailVerification = "REGISTRATION_EMAIL_VERIFICATION";
        public const string EvaluationInvitation = "EVALUATION_INVITATION";
        public const string EvaluationUpdate = "EVALUATION_UPDATE";

        /// <summary>Admin-authored custom notification (no template row).</summary>
        public const string CustomNotification = "CUSTOM_NOTIFICATION";
    }
}
