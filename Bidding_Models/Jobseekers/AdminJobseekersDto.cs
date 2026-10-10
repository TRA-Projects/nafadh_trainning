using System.Collections.Generic;

namespace Bidding_Models.Jobseekers
{
    /// <summary>
    /// Admin User Management → Jobseekers Directory. Row grain: one row = one `Profile`
    /// (ProfileType = JobSeeker). Deliberately small — this is an identity/browsing card, not a
    /// Reports-style export row (see the field discovery study). Does NOT carry Civil Number, phone,
    /// email, DOB, age, or gender — those stay inside the existing full Profile view, never the bulk
    /// Directory list, per the privacy-aware card design decision. Does NOT carry Qualification —
    /// removed per the card-refinement review (navigation to a person, not a mini-report).
    /// </summary>
    public class AdminJobseekerListRowDto
    {
        public int ProfileId { get; set; }

        public string NameEn { get; set; }

        public string NameAr { get; set; }

        /// <summary>Platform job seeker code (e.g. STU-MOE-2026-101) — formatted via the same
        /// `CredentialJobSeekerIdHelper.FormatJobSeekerId` every other jobseeker-code consumer in this
        /// codebase already uses (Credential Management, Evaluation Portfolio), not re-derived.</summary>
        public string JobSeekerCode { get; set; }

        /// <summary>False for MOL-imported job seekers who have not registered (no login) yet.</summary>
        public bool IsRegistered { get; set; } = true;

        /// <summary>Official/MOL-resolved governorate — via `EffectiveLocationResolver.
        /// TryResolveOfficialGovernorate`, never the profile's own Current/User-owned
        /// GovernorateId/WilayatId FK. Null when unresolved (never guessed).</summary>
        public string GovernorateNameEn { get; set; }

        public string GovernorateNameAr { get; set; }

        /// <summary>
        /// "Employment" | "Training" | "Hiring" | "None" — the CURRENT status from the shared rule
        /// (`JobseekerCurrentStatusResolver`), never `AdminDashboardDbReader.IsHireRecord`: currently
        /// employed, trainee, being hired (accepted offer or a contract not started yet), or jobseeker.
        /// </summary>
        public string CurrentEngagementType { get; set; }

        /// <summary>Company for the SAME current placement or accepted offer — via `JobsRepository.
        /// ResolveOwnerCompanyEn/Ar`, the platform's one company-display-name resolver. Null when
        /// `CurrentEngagementType == "None"`.</summary>
        public string CurrentCompanyNameEn { get; set; }

        public string CurrentCompanyNameAr { get; set; }

        /// <summary>`JobOpportunity.TitleEn/Ar` for the same current placement or accepted offer. Null when
        /// `CurrentEngagementType == "None"`.</summary>
        public string CurrentOpportunityTitleEn { get; set; }

        public string CurrentOpportunityTitleAr { get; set; }
    }

    /// <summary>Event codes in the Admin jobseeker "Requests and Decisions History" table.</summary>
    public static class AdminJobseekerDecisionEventTypes
    {
        /// <summary>A job offer whose current response status is Rejected.</summary>
        public const string OfferRejected = "offer_rejected";

        /// <summary>A jobseeker withdrawal request that was approved.</summary>
        public const string Withdrawal = "withdrawal";
    }

    /// <summary>One completed request/decision for a jobseeker (Admin only).</summary>
    public class AdminJobseekerDecisionHistoryEventDto
    {
        /// <summary>See <see cref="AdminJobseekerDecisionEventTypes"/>.</summary>
        public string EventType { get; set; }

        /// <summary>When the decision happened (offer response / withdrawal approval), ISO 8601.</summary>
        public string OccurredAtIso { get; set; }

        public int JobOpportunityId { get; set; }

        public string OpportunityTitleEn { get; set; }

        public string OpportunityTitleAr { get; set; }

        public string CompanyNameEn { get; set; }

        public string CompanyNameAr { get; set; }

        /// <summary>Persisted status code (offer response status or withdrawal request status).</summary>
        public string Status { get; set; }

        /// <summary>Catalog value key (<c>OFFR_RJCT</c> or <c>WD_RSN</c>); null when none was recorded.</summary>
        public string ReasonKey { get; set; }

        public string ReasonEn { get; set; }

        public string ReasonAr { get; set; }

        /// <summary>Free text entered by the jobseeker with the decision (offer notes / withdrawal reason).</summary>
        public string Details { get; set; }

        /// <summary>Withdrawals only: when the jobseeker submitted the request, ISO 8601 (the approval date
        /// is <see cref="OccurredAtIso"/>). Null for rejected offers.</summary>
        public string RequestedAtIso { get; set; }
    }

    public class AdminJobseekerDecisionHistoryDto
    {
        public int ProfileId { get; set; }

        /// <summary>Newest first.</summary>
        public List<AdminJobseekerDecisionHistoryEventDto> Events { get; set; } =
            new List<AdminJobseekerDecisionHistoryEventDto>();
    }

    /// <summary>Primary current-situation codes returned to the Admin profile (the UI localizes them).</summary>
    public static class AdminJobseekerSituationStatuses
    {
        public const string Employed = "employed";
        public const string Trainee = "trainee";
        public const string Hiring = "hiring";
        public const string Jobseeker = "jobseeker";
    }

    /// <summary>The current employment or training placement behind "employed" / "trainee".</summary>
    public class AdminJobseekerPlacementDto
    {
        public int JobOpportunityId { get; set; }

        public string TitleEn { get; set; }

        public string TitleAr { get; set; }

        public string CompanyNameEn { get; set; }

        public string CompanyNameAr { get; set; }

        /// <summary>Stored contract type key; labels come from the contract-type catalog.</summary>
        public string ContractTypeKey { get; set; }

        public string ContractTypeEn { get; set; }

        public string ContractTypeAr { get; set; }

        public string StartDateIso { get; set; }

        public string EndDateIso { get; set; }

        /// <summary>False when the contract start date is still in the future.</summary>
        public bool HasStarted { get; set; }

        /// <summary>True when the contract end date has passed while the placement is still open.</summary>
        public bool ContractPeriodEnded { get; set; }

        /// <summary>The final evaluation was submitted (placement status Evaluation Pending) but it is not closed.</summary>
        public bool FinalEvaluationSubmitted { get; set; }

        /// <summary>Stored final-evaluation outcome, when submitted.</summary>
        public string FinalEvaluationOutcome { get; set; }
    }

    /// <summary>An accepted offer whose contract is not registered yet ("hiring").</summary>
    public class AdminJobseekerHiringDto
    {
        public int JobOpportunityId { get; set; }

        public string TitleEn { get; set; }

        public string TitleAr { get; set; }

        public string CompanyNameEn { get; set; }

        public string CompanyNameAr { get; set; }

        public string OfferAcceptedAtIso { get; set; }
    }

    /// <summary>Admin only: the jobseeker's real current situation (read-only resolution).</summary>
    public class AdminJobseekerCurrentSituationDto
    {
        public int ProfileId { get; set; }

        /// <summary>See <see cref="AdminJobseekerSituationStatuses"/>.</summary>
        public string PrimaryStatus { get; set; }

        /// <summary>Set for employed / trainee.</summary>
        public AdminJobseekerPlacementDto Placement { get; set; }

        /// <summary>Set for hiring.</summary>
        public AdminJobseekerHiringDto Hiring { get; set; }

        /// <summary>Other current placements besides <see cref="Placement"/>.</summary>
        public int OtherActivePlacementCount { get; set; }

        /// <summary>Job offers waiting for the jobseeker's response.</summary>
        public int PendingOfferCount { get; set; }

        /// <summary>A jobseeker withdrawal request is still under review.</summary>
        public bool HasPendingWithdrawal { get; set; }
    }

    /// <summary>Kinds of item in the Admin profile's upcoming agenda.</summary>
    public static class AdminJobseekerAgendaItemKinds
    {
        /// <summary>An interview enrollment (InterviewEnrollment).</summary>
        public const string Interview = "interview";

        /// <summary>A credential assessment enrollment (CredentialEnrollment).</summary>
        public const string Assessment = "assessment";
    }

    /// <summary>
    /// One upcoming interview or assessment: an open enrollment scheduled today or later. Status is the
    /// persisted enrollment status code (the UI localizes it).
    /// </summary>
    public class AdminJobseekerAgendaItemDto
    {
        /// <summary>See <see cref="AdminJobseekerAgendaItemKinds"/>.</summary>
        public string Kind { get; set; }

        /// <summary>Enrollment reference.</summary>
        public string Reference { get; set; }

        /// <summary>Interview record / credential definition title.</summary>
        public string TitleEn { get; set; }

        public string TitleAr { get; set; }

        public string Status { get; set; }

        /// <summary>Stored evaluation date (wall-clock), ISO 8601.</summary>
        public string ScheduledAtIso { get; set; }

        /// <summary>Assessments only.</summary>
        public int? DurationMinutes { get; set; }

        /// <summary>Assessments only: credential definition kind.</summary>
        public string CredentialKind { get; set; }

        /// <summary>Interviews only.</summary>
        public int? RoundNumber { get; set; }

        /// <summary>Interviews only: stored interview type code.</summary>
        public string InterviewType { get; set; }

        /// <summary>Interviews only.</summary>
        public string LocationOrLink { get; set; }

        /// <summary>Interviews only: the interview record's opportunity reference, as stored.</summary>
        public string OpportunityReference { get; set; }

        /// <summary>Null when the opportunity reference does not match an opportunity.</summary>
        public int? JobOpportunityId { get; set; }

        public string OpportunityTitleEn { get; set; }

        public string OpportunityTitleAr { get; set; }

        public string CompanyNameEn { get; set; }

        public string CompanyNameAr { get; set; }
    }

    public class AdminJobseekerAgendaDto
    {
        /// <summary>All upcoming items; <see cref="Items"/> holds at most the agenda limit.</summary>
        public int TotalCount { get; set; }

        /// <summary>All upcoming interviews (counted before the limit) — the Admin "upcoming interviews" KPI.</summary>
        public int InterviewCount { get; set; }

        /// <summary>All upcoming assessments (counted before the limit).</summary>
        public int AssessmentCount { get; set; }

        /// <summary>Soonest upcoming interview date (wall-clock ISO), found before the limit; null when none.</summary>
        public string NextInterviewAtIso { get; set; }

        /// <summary>Soonest first.</summary>
        public List<AdminJobseekerAgendaItemDto> Items { get; set; } = new List<AdminJobseekerAgendaItemDto>();
    }

    /// <summary>The latest interview enrollment for an opportunity (matched by opportunity reference).</summary>
    public class AdminJobseekerOpportunityInterviewDto
    {
        public string InterviewRecordReference { get; set; }

        /// <summary>Persisted interview enrollment status code.</summary>
        public string Status { get; set; }

        public int RoundNumber { get; set; }

        public string ScheduledAtIso { get; set; }
    }

    /// <summary>
    /// The latest employment procedure for an opportunity, exactly as stored. Present only when an
    /// EmploymentProcedureRecord exists; each field is the persisted status code or timestamp.
    /// </summary>
    public class AdminJobseekerOpportunityEmploymentDto
    {
        public string OfferResponseStatus { get; set; }

        public string OfferSentAtIso { get; set; }

        /// <summary>When the jobseeker responded to the offer.</summary>
        public string OfferRespondedAtIso { get; set; }

        public string ContractStatus { get; set; }

        public string ContractRegisteredAtIso { get; set; }

        /// <summary>Employment procedure lifecycle status (Cancelled after an approved withdrawal).</summary>
        public string LifecycleStatus { get; set; }

        /// <summary>PostEmploymentRecord status; null when no placement record exists.</summary>
        public string PlacementStatus { get; set; }
    }

    /// <summary>
    /// Opportunity-history progression milestones, lowest to highest. Each is set only when a stored
    /// record proves it (see AdminJobseekerActivityOverviewReader.ResolveFurthestStage).
    /// </summary>
    public static class AdminJobseekerOpportunityStages
    {
        public const string Nominated = "nominated";
        public const string AskToApplySent = "ask_to_apply_sent";
        public const string Applied = "applied";
        public const string Shortlisted = "shortlisted";
        public const string InterviewReached = "interview_reached";
        public const string InterviewCompleted = "interview_completed";
        public const string OfferSent = "offer_sent";
        public const string OfferAccepted = "offer_accepted";
        public const string ContractRegistered = "contract_registered";
    }

    /// <summary>
    /// Confirmed closing results of an opportunity journey (see
    /// AdminJobseekerActivityOverviewReader.ResolveOutcome). Pending states never produce one.
    /// </summary>
    public static class AdminJobseekerOpportunityOutcomes
    {
        public const string AskToApplyDeclined = "ask_to_apply_declined";
        public const string ApplicationRejected = "application_rejected";
        public const string OfferRejected = "offer_rejected";
        public const string Withdrawn = "withdrawn";
        public const string EarlyTerminated = "early_terminated";
    }

    /// <summary>
    /// One jobseeker + opportunity participation with every stage the stored records prove. Nothing
    /// is inferred: a stage block is null when its record does not exist.
    /// </summary>
    public class AdminJobseekerOpportunityHistoryRowDto
    {
        /// <summary>
        /// Furthest confirmed progression milestone (<see cref="AdminJobseekerOpportunityStages"/>); null
        /// when no milestone is proven.
        /// </summary>
        public string FurthestStage { get; set; }

        /// <summary>Confirmed closing result (<see cref="AdminJobseekerOpportunityOutcomes"/>); null when none.</summary>
        public string Outcome { get; set; }

        public int JobOpportunityId { get; set; }

        public string JobReference { get; set; }

        /// <summary>Opportunity title, falling back to the job reference (same rule as JobsRepository).</summary>
        public string TitleEn { get; set; }

        public string TitleAr { get; set; }

        /// <summary>Null when the owner company cannot be resolved.</summary>
        public string CompanyNameEn { get; set; }

        public string CompanyNameAr { get; set; }

        /// <summary>Stored application type code.</summary>
        public string ApplicationType { get; set; }

        public bool IsNominated { get; set; }

        public bool IsTraNominated { get; set; }

        public bool NominatedByTra { get; set; }

        public bool AskToApplySent { get; set; }

        public bool HasApplied { get; set; }

        public bool IsShortlisted { get; set; }

        /// <summary>Stored pipeline status code.</summary>
        public string PipelineStatus { get; set; }

        /// <summary>Catalog key of the jobseeker's ask-to-apply decline reason, when recorded.</summary>
        public string AskToApplyDeclineReasonKey { get; set; }

        public string AskToApplyDeclineReasonEn { get; set; }

        public string AskToApplyDeclineReasonAr { get; set; }

        public int? MatchPercent { get; set; }

        /// <summary>When the participation record was created (first nomination, ask or application).</summary>
        public string CreatedAtIso { get; set; }

        public string AppliedOnIso { get; set; }

        /// <summary>Last change to the participation record.</summary>
        public string UpdatedAtIso { get; set; }

        public AdminJobseekerOpportunityInterviewDto Interview { get; set; }

        public AdminJobseekerOpportunityEmploymentDto Employment { get; set; }
    }

    /// <summary>
    /// One submitted/completed evaluation. <see cref="OverallScore"/> is the raw stored score of its own
    /// instrument; no source guarantees a common scale, so it is not normalized or compared across types.
    /// </summary>
    public class AdminJobseekerEvaluationHistoryRowDto
    {
        public int AssessmentResultId { get; set; }

        /// <summary>AssessmentSourceTypes code.</summary>
        public string SourceType { get; set; }

        /// <summary>
        /// AssessmentResult status code, as stored. This is NOT the evaluation's lifecycle: a TRA result
        /// stays "Submitted" after results are sent, and an interview result is "Completed" as soon as the
        /// seat evaluation is saved — see <see cref="LifecycleStatus"/>.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Authoritative lifecycle status of the record that owns the result, as stored: the credential
        /// enrollment status for a TRA assessment (Pending Submission until "Send results", then
        /// Completed), the current interview seat status for an interview evaluation (same seat as
        /// <see cref="EnrollmentReference"/>), the periodic evaluation status for a periodic review.
        /// Null when that record cannot be found.
        /// </summary>
        public string LifecycleStatus { get; set; }

        public string CompletedAtIso { get; set; }

        /// <summary>Raw stored score; null when none was recorded.</summary>
        public decimal? OverallScore { get; set; }

        /// <summary>Stored final decision code (interview evaluations).</summary>
        public string FinalDecision { get; set; }

        public string RejectionReasonKey { get; set; }

        public string Recommendations { get; set; }

        public string Feedback { get; set; }

        /// <summary>Interview record / credential definition title; null for periodic reviews.</summary>
        public string TitleEn { get; set; }

        public string TitleAr { get; set; }

        /// <summary>Credential assessments only.</summary>
        public string CredentialKind { get; set; }

        /// <summary>Interview evaluations only.</summary>
        public string InterviewType { get; set; }

        /// <summary>Interview evaluations only: the interview record's opportunity reference, as stored.</summary>
        public string OpportunityReference { get; set; }

        /// <summary>
        /// Enrollment reference that opens the full result (same rule as the Assessment Results tab):
        /// credential assessment → its CredentialEnrollment; interview → the profile's current seat on
        /// that interview record. Null for periodic reviews and whenever no enrollment exists.
        /// </summary>
        public string EnrollmentReference { get; set; }

        /// <summary>Interview evaluations (matched by reference) and periodic reviews (via the placement).</summary>
        public int? JobOpportunityId { get; set; }

        public string OpportunityTitleEn { get; set; }

        public string OpportunityTitleAr { get; set; }

        public string CompanyNameEn { get; set; }

        public string CompanyNameAr { get; set; }

        /// <summary>Periodic reviews only: stored period code and range.</summary>
        public string Period { get; set; }

        /// <summary>
        /// Periodic reviews only: the post-employment record reference — the <c>:id</c> of the existing
        /// Post-Employment periodic-evaluation detail route. Null for other types.
        /// </summary>
        public string PostEmploymentRecordReference { get; set; }

        /// <summary>
        /// Periodic reviews only: the periodic evaluation's display id — the <c>:evalId</c> of that route,
        /// built by PostEmploymentRepository.PeriodicEvaluationDisplayId. Null for other types.
        /// </summary>
        public string PeriodicEvaluationId { get; set; }

        public string PeriodFromIso { get; set; }

        public string PeriodToIso { get; set; }
    }

    /// <summary>Admin only: the profile's upcoming agenda plus the first page of each history.</summary>
    public class AdminJobseekerActivityOverviewDto
    {
        public int ProfileId { get; set; }

        public AdminJobseekerAgendaDto Agenda { get; set; }

        /// <summary>Most recently updated first.</summary>
        public PagedListResponseDto<AdminJobseekerOpportunityHistoryRowDto> Opportunities { get; set; }

        /// <summary>Most recently completed first.</summary>
        public PagedListResponseDto<AdminJobseekerEvaluationHistoryRowDto> Evaluations { get; set; }
    }
}
