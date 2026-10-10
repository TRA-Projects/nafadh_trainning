namespace Bidding_Models.Jobseekers
{
    /// <summary>
    /// H6: ported verbatim from Asala's export branch (source commit c69ca316). Caller identity derived
    /// from JWT claims — never supplied by the client request body. Deliberately plain data: no
    /// HttpContext/controller coupling lives here or in <c>AdminDashboardExportService</c> — extracting
    /// these two values from <c>HttpContext.User.Claims</c> remains the controller's job, deferred to
    /// H7. Both fields are optional (default <c>null</c>), so the service/audit logger work correctly
    /// even before a controller ever supplies one.
    /// </summary>
    public sealed class AdminDashboardExportCallerContext
    {
        public int? ProfileId { get; set; }
        public int? VendorType { get; set; }
    }
}
