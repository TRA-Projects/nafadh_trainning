using System.Collections.Generic;

namespace Bidding_Models.Jobseekers
{
    // -------------------------------------------------------------------------
    // Shared filter & metadata (all GET /api/jobseekers/admin/dashboard/*)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Query filter bound from <c>[FromQuery]</c> on every admin dashboard endpoint.
    /// </summary>
    public class AdminDashboardFilterDto
    {
        /// <summary><c>all</c> or a calendar year (<c>YYYY</c>).</summary>
        public string year { get; set; } = "all";

        /// <summary>Inclusive start date (<c>YYYY-MM-DD</c>).</summary>
        public string dateFrom { get; set; }

        /// <summary>Inclusive end date (<c>YYYY-MM-DD</c>).</summary>
        public string dateTo { get; set; }

        /// <summary><c>all</c> or a normalized major / specialization key.</summary>
        public string specialization { get; set; } = "all";

        /// <summary><c>all</c>, <c>accepted</c>, <c>pending</c>, or <c>notAccepted</c>.</summary>
        public string employmentStatus { get; set; } = "all";

        /// <summary><c>all</c>, <c>TRA</c>, or <c>NON_TRA</c> — <see cref="Bidding_Models.DatabaseModel.JobOpportunities.JobOpportunity.NominationType"/>.</summary>
        public string opportunityType { get; set; } = "all";

        /// <summary><c>all</c> or a <see cref="Bidding_Models.DatabaseModel.JobOpportunities.JobOpportunityStatuses"/> value.</summary>
        public string opportunityStatus { get; set; } = "all";

        /// <summary>
        /// <c>all</c>, a <c>WRK_SCOPE</c> <c>SystemConfigValue.ValueKey</c> (e.g. <c>information-technology</c>),
        /// or the numeric <c>SystemConfigValue.Id</c> — same value shape accepted by the opportunity
        /// create/edit form's own category selector. Maps to
        /// <see cref="Bidding_Models.DatabaseModel.JobOpportunities.JobOpportunity.CategoryValueKey"/>
        /// ("Opportunity Field" / مجال الفرصة).
        /// </summary>
        public string opportunityField { get; set; } = "all";

        /// <summary>
        /// <c>all</c> or a 4-digit calendar year (<c>YYYY</c>) — resolved per jobseeker from
        /// <see cref="Bidding_Models.DatabaseModel.JobseekerProfile.JobSeekerEducationSummary.YearOfGraduation"/>
        /// (preferred) or the latest <see cref="Bidding_Models.DatabaseModel.JobseekerProfile.JobSeekerEducation.EndYear"/>.
        /// A jobseeker-population dimension only — never narrows a company/opportunity population.
        /// </summary>
        public string graduationYear { get; set; } = "all";

        /// <summary>
        /// <c>all</c>, <c>male</c>, or <c>female</c>. Jobseeker-population dimension only.
        /// Matched from <c>ProfileMOLSummaries.Gender</c>, then <c>Profiles.Gender</c>
        /// (<c>male</c>/<c>ذكر</c>, <c>female</c>/<c>أنثى</c>). Empty gender matches neither.
        /// </summary>
        public string gender { get; set; } = "all";

        /// <summary>
        /// Dashboard switcher: <c>jobs</c> (default — excludes posted training/internship) or
        /// <c>training</c> (only <c>JobTypeValueKey</c> <c>training</c> and <c>internship</c>).
        /// </summary>
        public string jobTypeScope { get; set; } = "jobs";
    }

    /// <summary>Resolved inclusive date window after defaults and year intersection.</summary>
    public class AdminDashboardDateRangeDto
    {
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
        public string year { get; set; }
    }

    /// <summary>Response envelope metadata echoed on every dashboard payload.</summary>
    public class AdminDashboardMetaDto
    {
        public string generatedAtUtc { get; set; }
        public AdminDashboardFilterDto appliedFilters { get; set; }
        public AdminDashboardDateRangeDto effectiveDateRange { get; set; }
    }

    // -------------------------------------------------------------------------
    // GET …/summary
    // -------------------------------------------------------------------------

    /// <summary>KPI card counts for the admin dashboard header row.</summary>
    public class AdminDashboardSummaryKpisDto
    {
        public int totalOpportunities { get; set; }
        public int totalJobSeekers { get; set; }
        public int completedAssessments { get; set; }
        public int hiresThroughSystem { get; set; }
    }

    /// <summary>Response body for <c>GET /api/jobseekers/admin/dashboard/summary</c>.</summary>
    public class AdminDashboardSummaryResponseDto
    {
        public AdminDashboardMetaDto meta { get; set; }
        public AdminDashboardSummaryKpisDto kpis { get; set; }
    }

    // -------------------------------------------------------------------------
    // GET …/trends
    // -------------------------------------------------------------------------

    /// <summary>One time bucket in the activity trend line chart.</summary>
    public class AdminDashboardTrendPointDto
    {
        /// <summary>Bucket key: <c>YYYY-MM-DD</c> (day) or <c>YYYY-MM</c> (month).</summary>
        public string bucketKey { get; set; }

        public string labelEn { get; set; }
        public string labelAr { get; set; }

        /// <summary>Opportunities that became Published in this bucket (via PostedDate — set exactly once,
        /// the moment status first transitions to Published; see <c>JobsRepository.UpdateJobStatusAsync</c>).</summary>
        public int opportunities { get; set; }
        public int interviews { get; set; }
        public int assessments { get; set; }

        /// <summary>Submitted applications (HasApplied == true) in this bucket, via AppliedOn.</summary>
        public int applications { get; set; }
    }

    /// <summary>Activity trend series (opportunities, interviews, assessments).</summary>
    public class AdminDashboardTrendsDto
    {
        /// <summary><c>day</c> when the range is short; otherwise <c>month</c>.</summary>
        public string granularity { get; set; }

        public List<AdminDashboardTrendPointDto> points { get; set; } = new List<AdminDashboardTrendPointDto>();
    }

    /// <summary>Response body for <c>GET /api/jobseekers/admin/dashboard/trends</c>.</summary>
    public class AdminDashboardTrendsResponseDto
    {
        public AdminDashboardMetaDto meta { get; set; }
        public AdminDashboardTrendsDto trends { get; set; }
    }

    // -------------------------------------------------------------------------
    // GET …/jobseekers-by-specialization
    // -------------------------------------------------------------------------

    /// <summary>One major / specialization bucket in the horizontal bar chart.</summary>
    public class AdminDashboardSpecializationBucketDto
    {
        public string majorKey { get; set; }
        public string majorEn { get; set; }
        public string majorAr { get; set; }
        public int count { get; set; }

        /// <summary>Percentage of the top bucket count (top bucket = 100).</summary>
        public int percentOfTop { get; set; }

        public int rank { get; set; }
    }

    /// <summary>Response body for <c>GET /api/jobseekers/admin/dashboard/jobseekers-by-specialization</c>.</summary>
    public class AdminDashboardJobSeekersBySpecializationResponseDto
    {
        public AdminDashboardMetaDto meta { get; set; }

        public List<AdminDashboardSpecializationBucketDto> buckets { get; set; } =
            new List<AdminDashboardSpecializationBucketDto>();

        public int totalJobSeekers { get; set; }
    }

    // -------------------------------------------------------------------------
    // GET …/interviews-by-status  &  GET …/employment-by-response
    // -------------------------------------------------------------------------

    /// <summary>Generic labelled count bucket for donut / bar breakdown charts.</summary>
    public class AdminDashboardStatusBucketDto
    {
        public string bucketKey { get; set; }
        public string labelEn { get; set; }
        public string labelAr { get; set; }
        public int count { get; set; }
    }

    /// <summary>Response body for <c>GET /api/jobseekers/admin/dashboard/interviews-by-status</c>.</summary>
    public class AdminDashboardInterviewsByStatusResponseDto
    {
        public AdminDashboardMetaDto meta { get; set; }

        public List<AdminDashboardStatusBucketDto> buckets { get; set; } =
            new List<AdminDashboardStatusBucketDto>();

        public int totalInterviews { get; set; }
    }

    /// <summary>Response body for <c>GET /api/jobseekers/admin/dashboard/employment-by-response</c>.</summary>
    public class AdminDashboardEmploymentByResponseResponseDto
    {
        public AdminDashboardMetaDto meta { get; set; }

        public List<AdminDashboardStatusBucketDto> buckets { get; set; } =
            new List<AdminDashboardStatusBucketDto>();

        public int totalRecords { get; set; }
    }

    // -------------------------------------------------------------------------
    // GET …/top-companies
    // -------------------------------------------------------------------------

    /// <summary>One row in the top-companies leaderboard.</summary>
    public class AdminDashboardTopCompanyRowDto
    {
        public int ownerProfileId { get; set; }
        public string companyKey { get; set; }
        public string companyNameEn { get; set; }
        public string companyNameAr { get; set; }
        public int opportunitiesPosted { get; set; }
        public int interviewSlots { get; set; }
        public int employmentRecords { get; set; }
        public int totalActivity { get; set; }
        public int rank { get; set; }
    }

    /// <summary>Response body for <c>GET /api/jobseekers/admin/dashboard/top-companies</c>.</summary>
    public class AdminDashboardTopCompaniesResponseDto
    {
        public AdminDashboardMetaDto meta { get; set; }

        public List<AdminDashboardTopCompanyRowDto> companies { get; set; } =
            new List<AdminDashboardTopCompanyRowDto>();
    }

    // -------------------------------------------------------------------------
    // GET …/jobseekers-by-wilayat
    // -------------------------------------------------------------------------

    /// <summary>Job seeker count for one Wilayat (geo map / table).</summary>
    public class AdminDashboardWilayatBucketDto
    {
        /// <summary>Stable map key for ECharts GEO (e.g. <c>W-12</c>).</summary>
        public string wilayatKey { get; set; }

        public int wilayatId { get; set; }
        public string wilayatNameEn { get; set; }
        public string wilayatNameAr { get; set; }
        public int governorateId { get; set; }
        public string governorateNameEn { get; set; }
        public string governorateNameAr { get; set; }
        public int jobSeekerCount { get; set; }

        /// <summary>Subset of jobSeekerCount matching IsHireRecord — Phase C3, added so the future Oman
        /// GeoJSON map phase is presentation-only, not another data contract change.</summary>
        public int employedJobSeekerCount { get; set; }
    }

    /// <summary>Response body for <c>GET /api/jobseekers/admin/dashboard/jobseekers-by-wilayat</c>.</summary>
    public class AdminDashboardJobSeekersByWilayatResponseDto
    {
        public AdminDashboardMetaDto meta { get; set; }

        public List<AdminDashboardWilayatBucketDto> wilayats { get; set; } =
            new List<AdminDashboardWilayatBucketDto>();

        public int totalJobSeekers { get; set; }

        /// <summary>Profiles with no resolvable Wilayat assignment.</summary>
        public int unassignedCount { get; set; }
    }

    // -------------------------------------------------------------------------
    // GET …/kpi-overview
    // -------------------------------------------------------------------------

    /// <summary>
    /// Current-state Jobseekers-vs-Employed figures. "Employed" = distinct job seeker profiles with an
    /// <c>EmploymentProcedureRecord</c> where <c>OfferResponseStatus == Accepted</c>,
    /// <c>LifecycleStatus != Cancelled</c>, and (<c>LifecycleStatus == Completed</c> OR
    /// <c>ContractStatus == Registered</c>) — the same predicate as the platform's existing
    /// <c>IsHireRecord</c> convention, applied without any date-window restriction.
    /// </summary>
    public class AdminDashboardJobSeekersVsEmployedDto
    {
        public int totalJobSeekers { get; set; }
        public int employedJobSeekers { get; set; }

        /// <summary><c>employedJobSeekers / totalJobSeekers * 100</c>, rounded to 1 decimal; 0 when totalJobSeekers is 0.</summary>
        public decimal employmentPercentage { get; set; }
    }

    /// <summary>
    /// Response body for <c>GET /api/jobseekers/admin/dashboard/kpi-overview</c>.
    /// Deliberately separate from <c>summary</c>: Jobseekers vs Employed / Published Opportunities /
    /// Registered Companies are current-state by business definition and never consult the resolved
    /// date window; Total Applications is the one date-aware KPI here and always honors the resolved
    /// window in <c>meta.effectiveDateRange</c> (Task 1, Phase C3 — the visible filter-panel date range
    /// is a genuinely active filter, not a silently-ignored default).
    /// </summary>
    public class AdminDashboardKpiOverviewResponseDto
    {
        public AdminDashboardMetaDto meta { get; set; }
        public AdminDashboardJobSeekersVsEmployedDto jobseekersVsEmployed { get; set; }

        /// <summary><c>Status == Published</c> AND (no <c>ExpiryDate</c> or not yet expired) — narrowed by
        /// <c>opportunityType</c> when supplied. Current-state; not date-window-scoped.</summary>
        public int publishedOpportunities { get; set; }

        /// <summary>Distinct root company profiles (<c>ProfileType == Operator</c>). OperatorUser (OUser)
        /// sub-accounts are never counted separately. Current-state.</summary>
        public int registeredCompanies { get; set; }

        /// <summary><c>HasApplied == true</c>, always scoped to <c>meta.effectiveDateRange</c> via <c>AppliedOn</c>.</summary>
        public int totalApplications { get; set; }

        /// <summary>vs. the immediately preceding period of equal duration to <c>meta.effectiveDateRange</c>
        /// (cumulative "as of" comparison — see <see cref="AdminDashboardDbReader.GetKpiOverviewCountsAsync"/>).</summary>
        public AdminDashboardKpiComparisonDto totalJobSeekersComparison { get; set; }

        /// <summary>Percentage-POINT difference (not relative %) between this period's and the previous
        /// period's employment ratio — the more honest representation of a ratio-vs-ratio change.</summary>
        public AdminDashboardKpiComparisonDto employmentPercentageComparison { get; set; }

        public AdminDashboardKpiComparisonDto registeredCompaniesComparison { get; set; }

        /// <summary>Always "unavailable": <c>JobOpportunity.Status</c> has no change history, so "was this
        /// opportunity Published-and-unexpired at a past date" cannot be reliably reconstructed for a
        /// snapshot metric like this one — reported explicitly rather than approximated.</summary>
        public AdminDashboardKpiComparisonDto publishedOpportunitiesComparison { get; set; }

        /// <summary>Distinct jobseekers with >=1 AssessmentResult in {Completed, Submitted} — the exact
        /// same query as the Evaluation Overview widget's own <c>evaluatedJobSeekers</c> (Phase KPI V2,
        /// KPI 4). Current-state, matching that widget's own already-established meaning — never
        /// scoped to the dashboard date filter.</summary>
        public int evaluatedJobSeekers { get; set; }
    }

    /// <summary>
    /// Growth/comparison indicator for one KPI vs. the immediately preceding period of equal duration.
    /// "percent" = safe relative-percentage change (<see cref="value"/> in percent, zero-baseline
    /// handled per <c>kind</c>); "pointDelta" = percentage-POINT difference (used only for the
    /// employment ratio); "unavailable" = no reliable historical basis exists — the UI must show a
    /// neutral state, never an invented percentage.
    /// </summary>
    public class AdminDashboardKpiComparisonDto
    {
        public string kind { get; set; }
        public decimal? value { get; set; }

        /// <summary>"up" | "down" | "flat" | "unavailable".</summary>
        public string direction { get; set; }
    }

    // -------------------------------------------------------------------------
    // GET …/top-companies-by-opportunities
    // -------------------------------------------------------------------------

    /// <summary>One root company ranked by opportunities it has offered (Published and later statuses).</summary>
    public class AdminDashboardTopCompanyByOpportunitiesRowDto
    {
        public int profileId { get; set; }
        public string companyKey { get; set; }
        public string companyNameEn { get; set; }
        public string companyNameAr { get; set; }
        public int publishedOpportunitiesCount { get; set; }
        public int rank { get; set; }
    }

    /// <summary>
    /// Response body for <c>GET /api/jobseekers/admin/dashboard/top-companies-by-opportunities</c>.
    /// A single, transparent ranking metric (offered-opportunity count, ParentProfileId-resolved to
    /// each company's root profile) — deliberately separate from the older <c>top-companies</c> blended
    /// activity leaderboard, which mixes opportunities/interviews/employment and is not
    /// ParentProfileId-aware. That endpoint is left untouched for any other existing consumer.
    /// </summary>
    public class AdminDashboardTopCompaniesByOpportunitiesResponseDto
    {
        public AdminDashboardMetaDto meta { get; set; }

        public List<AdminDashboardTopCompanyByOpportunitiesRowDto> companies { get; set; } =
            new List<AdminDashboardTopCompanyByOpportunitiesRowDto>();
    }

    // -------------------------------------------------------------------------
    // GET …/evaluation-overview
    // -------------------------------------------------------------------------

    /// <summary>
    /// Response body for <c>GET /api/jobseekers/admin/dashboard/evaluation-overview</c>.
    /// "Evaluated" = distinct job seeker profiles with at least one <c>AssessmentResult</c> whose
    /// Status is Completed or Submitted (re-verified against live data this phase). No "eligibility"
    /// concept exists in the schema — every registered jobseeker is in the denominator.
    /// </summary>
    public class AdminDashboardEvaluationOverviewResponseDto
    {
        public AdminDashboardMetaDto meta { get; set; }
        public int totalJobSeekers { get; set; }
        public int evaluatedJobSeekers { get; set; }
        public int notEvaluatedJobSeekers { get; set; }
    }

    // -------------------------------------------------------------------------
    // GET …/employment-status-overview
    // -------------------------------------------------------------------------

    /// <summary>
    /// Response body for <c>GET /api/jobseekers/admin/dashboard/employment-status-overview</c>
    /// (Phase KPI V2, Decision 4). Population = every registered jobseeker (same definition as KPI 1).
    /// Exhaustive two-way split — <c>currentlyJobSeekingJobSeekers</c> is always
    /// <c>totalRegisteredJobSeekers - employedJobSeekers</c>, never a separately-queried bucket, so
    /// <c>employedJobSeekers + currentlyJobSeekingJobSeekers == totalRegisteredJobSeekers</c> always
    /// holds — there is no "unknown" state to report. "Employed" reuses the platform's IsHireRecord
    /// predicate plus an explicit training-opportunity exclusion (verified against live data before
    /// implementing — see <c>AdminDashboardDbReader.GetEmploymentStatusOverviewAsync</c>).
    /// </summary>
    public class AdminDashboardEmploymentStatusOverviewResponseDto
    {
        public AdminDashboardMetaDto meta { get; set; }
        public int totalRegisteredJobSeekers { get; set; }
        public int employedJobSeekers { get; set; }
        public int currentlyJobSeekingJobSeekers { get; set; }

        /// <summary>Rounded to 1 decimal; 0 when totalRegisteredJobSeekers is 0.</summary>
        public decimal employedPercentage { get; set; }

        public decimal currentlyJobSeekingPercentage { get; set; }
    }

    // -------------------------------------------------------------------------
    // GET …/jobseekers-by-governorate
    // -------------------------------------------------------------------------

    /// <summary>One Governorate row (Oman Governorate Map V2). Always present for all 11 governorates,
    /// even when <c>totalRegisteredJobSeekers</c> is 0 — the map must never geographically "lose" a
    /// region just because it currently has no data.</summary>
    public class AdminDashboardGovernorateOverviewRowDto
    {
        public int governorateId { get; set; }
        public string governorateNameEn { get; set; }
        public string governorateNameAr { get; set; }
        public int totalRegisteredJobSeekers { get; set; }
        public int employed { get; set; }
        public int currentlyJobseeking { get; set; }
    }

    /// <summary>
    /// Response body for <c>GET /api/jobseekers/admin/dashboard/jobseekers-by-governorate</c> (Oman
    /// Governorate Map V2). Server-side aggregation only — the frontend never receives platform-wide
    /// profile rows to aggregate itself. <c>unmappedJobSeekers</c> is the count of registered
    /// jobseekers whose EffectiveGovernorate could not be resolved by any tier (see
    /// <c>Bidding_DLL.Helpers.Geography.EffectiveLocationResolver.TryResolveEffectiveGovernorate</c>) — never silently
    /// folded into any governorate's total.
    /// </summary>
    public class AdminDashboardGovernorateOverviewResponseDto
    {
        public AdminDashboardMetaDto meta { get; set; }
        public List<AdminDashboardGovernorateOverviewRowDto> governorates { get; set; } =
            new List<AdminDashboardGovernorateOverviewRowDto>();
        public int totalRegisteredJobSeekers { get; set; }
        public int unmappedJobSeekers { get; set; }
    }

    /// <summary>
    /// Candidate-rejection reason aggregates for Admin Excel.
    /// Counts <c>AssessmentResults</c> where <c>SourceType = JobPosting</c> and
    /// <c>FinalDecision = Rejected</c>. Labels come from live <c>RJCT_RSN</c> / <c>RJCT_SUB</c>
    /// catalog rows. Does not include offer refusal by the jobseeker.
    /// </summary>
    public class AdminDashboardJobseekerRejectionReasonsResponseDto
    {
        public AdminDashboardMetaDto meta { get; set; }

        public int totalEvents { get; set; }

        public List<AdminDashboardJobseekerRejectionReasonRowDto> rows { get; set; } =
            new List<AdminDashboardJobseekerRejectionReasonRowDto>();
    }

    public class AdminDashboardJobseekerRejectionReasonRowDto
    {
        public string reasonLabelEn { get; set; }
        public string reasonLabelAr { get; set; }
        public string subReasonLabelEn { get; set; }
        public string subReasonLabelAr { get; set; }
        public bool hasSubReason { get; set; }
        public int count { get; set; }

        /// <summary>Share of <see cref="AdminDashboardJobseekerRejectionReasonsResponseDto.totalEvents"/>, rounded to 1 decimal.</summary>
        public decimal percentage { get; set; }
    }

    // -------------------------------------------------------------------------
    // GET …/training-overview
    // -------------------------------------------------------------------------

    public class AdminDashboardNamedCountRowDto
    {
        public string key { get; set; }
        public string nameEn { get; set; }
        public string nameAr { get; set; }
        public int count { get; set; }
    }

    /// <summary>
    /// Response body for <c>GET /api/jobseekers/admin/dashboard/training-overview</c>.
    /// Training + internship postings plus profile training programs (<c>TRNG_TYPE</c> / skills).
    /// </summary>
    public class AdminDashboardTrainingOverviewResponseDto
    {
        public AdminDashboardMetaDto meta { get; set; }
        public int offeredOpportunities { get; set; }
        public int availableOpportunities { get; set; }
        public int jobSeekersWithTraining { get; set; }
        public int companiesOfferingTraining { get; set; }
        public List<AdminDashboardNamedCountRowDto> programTypes { get; set; } =
            new List<AdminDashboardNamedCountRowDto>();
        public List<AdminDashboardNamedCountRowDto> topSkills { get; set; } =
            new List<AdminDashboardNamedCountRowDto>();
        public List<AdminDashboardNamedCountRowDto> opportunityTypes { get; set; } =
            new List<AdminDashboardNamedCountRowDto>();
        public List<AdminDashboardNamedCountRowDto> fields { get; set; } =
            new List<AdminDashboardNamedCountRowDto>();
    }

    // -------------------------------------------------------------------------
    // GET …/contract-outcomes
    // -------------------------------------------------------------------------

    public class AdminDashboardContractOperatorRowDto
    {
        public int profileId { get; set; }
        public string nameEn { get; set; }
        public string nameAr { get; set; }
        public int count { get; set; }
    }

    /// <summary>Decline reason with the operator company that accounts for the most of those rejections.</summary>
    public class AdminDashboardContractOutcomeReasonRowDto
    {
        public string key { get; set; }
        public string nameEn { get; set; }
        public string nameAr { get; set; }
        public int count { get; set; }
        public int operatorProfileId { get; set; }
        public string operatorNameEn { get; set; }
        public string operatorNameAr { get; set; }
        public int operatorCount { get; set; }
    }

    /// <summary>
    /// Offers, withdrawals, and early terminations for the dashboard date range.
    /// Reason rankings use the category chosen when the request or decline was submitted.
    /// </summary>
    public class AdminDashboardContractOutcomesResponseDto
    {
        public AdminDashboardMetaDto meta { get; set; }
        public int offersSent { get; set; }
        public int offersAccepted { get; set; }
        public int offersRejected { get; set; }
        public int contractsRegistered { get; set; }
        public int withdrawalsSubmitted { get; set; }
        public int withdrawalsApproved { get; set; }
        public int withdrawalsRejected { get; set; }
        public int terminationsSubmitted { get; set; }
        public int terminationsApproved { get; set; }
        public int terminationsRejected { get; set; }
        public List<AdminDashboardContractOperatorRowDto> topOfferingOperators { get; set; } =
            new List<AdminDashboardContractOperatorRowDto>();
        public List<AdminDashboardContractOutcomeReasonRowDto> offerRejectionReasons { get; set; } =
            new List<AdminDashboardContractOutcomeReasonRowDto>();
        public List<AdminDashboardNamedCountRowDto> withdrawalReasons { get; set; } =
            new List<AdminDashboardNamedCountRowDto>();
        public List<AdminDashboardNamedCountRowDto> terminationReasons { get; set; } =
            new List<AdminDashboardNamedCountRowDto>();
        public List<AdminDashboardContractOperatorRowDto> topTerminatingOperators { get; set; } =
            new List<AdminDashboardContractOperatorRowDto>();
    }

    /// <summary>One currently published opportunity with its actual applicants and their average match.</summary>
    public class AdminDashboardOpportunityApplicantMatchRowDto
    {
        public int jobOpportunityId { get; set; }
        public string jobReference { get; set; }
        public string titleEn { get; set; }
        public string titleAr { get; set; }
        /// <summary>Participations with <c>HasApplied</c>.</summary>
        public int applicantCount { get; set; }
        /// <summary>Whole-number average Match %. Null when there are no applicants or the opportunity
        /// has no required skills to match against.</summary>
        public int? averageMatchPercent { get; set; }
    }

    /// <summary>GET /api/jobseekers/admin/dashboard/opportunity-applicant-match</summary>
    public class AdminDashboardOpportunityApplicantMatchResponseDto
    {
        public AdminDashboardMetaDto meta { get; set; }
        public List<AdminDashboardOpportunityApplicantMatchRowDto> opportunities { get; set; } =
            new List<AdminDashboardOpportunityApplicantMatchRowDto>();
    }
}
