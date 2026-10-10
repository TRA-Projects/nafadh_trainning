namespace Bidding_Models.Jobseekers
{
    /// <summary>
    /// H4: pure, data-only container for everything a server-side Excel workbook needs to render —
    /// ported from Asala's <c>IAdminDashboardExportDataProvider.cs</c> (only this data class, not the
    /// provider interface itself, which belongs to the not-yet-ported data-assembly layer). Every
    /// property is an already-existing response DTO type (the exact same ones the live dashboard GET
    /// endpoints already return) — no DB context, no service logic, no HTTP objects. Nothing populates
    /// this yet in this branch; a future phase that ports the data-assembly layer is responsible for
    /// filling it in by calling the existing <c>IAdminDashboardService</c> methods.
    /// </summary>
    public sealed class AdminDashboardExportDataSet
    {
        public AdminDashboardKpiOverviewResponseDto KpiOverview { get; set; }
        public AdminDashboardTrendsResponseDto Trends { get; set; }
        public AdminDashboardEmploymentStatusOverviewResponseDto EmploymentStatus { get; set; }
        public AdminDashboardGovernorateOverviewResponseDto Governorates { get; set; }
        public AdminDashboardTopCompaniesByOpportunitiesResponseDto TopCompanies { get; set; }
        /// <summary>Jobs scope only — null in a Training-scope export, where the dashboard panel does not exist.</summary>
        public AdminDashboardOpportunityApplicantMatchResponseDto OpportunityApplicantMatch { get; set; }
        public AdminDashboardJobSeekersBySpecializationResponseDto Specialization { get; set; }
        public AdminDashboardEvaluationOverviewResponseDto EvaluationOverview { get; set; }
        public AdminDashboardEmploymentByResponseResponseDto EmploymentProcedureStatus { get; set; }
        public AdminDashboardJobseekerRejectionReasonsResponseDto JobseekerRejectionReasons { get; set; }
    }
}
