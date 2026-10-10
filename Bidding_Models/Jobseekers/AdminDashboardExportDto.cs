using System;

namespace Bidding_Models.Jobseekers
{
    /// <summary>
    /// POST body shape for the future admin dashboard export endpoint (H2: request-validation
    /// prerequisite only — no controller/endpoint exists yet in this branch).
    /// </summary>
    public class AdminDashboardExportRequestDto
    {
        /// <summary>Supported values: <c>excel</c>, <c>pdf</c>.</summary>
        public string format { get; set; }

        /// <summary><c>en</c> or <c>ar</c>.</summary>
        public string locale { get; set; }

        /// <summary>Same filter shape as the existing dashboard GET endpoints.</summary>
        public AdminDashboardFilterDto filters { get; set; }
    }

    /// <summary>
    /// H6: server-side export artifact returned by <c>IAdminDashboardExportService.ExportAsync</c> —
    /// plain data only, no <c>IActionResult</c>/HTTP concept. The eventual controller (H7) is
    /// responsible for turning this into an actual file response.
    /// </summary>
    public sealed class AdminDashboardExportResultDto
    {
        public byte[] content { get; set; }
        public string contentType { get; set; }
        public string fileName { get; set; }
        public DateTime generatedAtUtc { get; set; }
    }
}
