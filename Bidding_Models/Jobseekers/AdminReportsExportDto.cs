using System.Collections.Generic;

namespace Bidding_Models.Jobseekers
{
    /// <summary>
    /// R4 (Admin Reports Excel Export) — deliberately mirrors <see cref="AdminReportsPreviewRequestDto"/>'s
    /// shared fields (<c>reportId</c>/<c>fields</c>/<c>filterValues</c>/<c>locale</c>) rather than
    /// inheriting from it: export has no <c>page</c>/<c>pageSize</c> (it exports the whole approved
    /// filtered dataset, bounded server-side — see <c>AdminReportsExportService</c>'s row-limit check,
    /// not a client-chosen page) and adds <c>format</c>, which preview has no use for. Same whitelist
    /// discipline as preview: <c>fields</c>/<c>filterValues</c> keys are opaque strings the server
    /// alone resolves via <c>AdminReportsFieldRegistry</c> — never raw columns/SQL.
    /// </summary>
    public sealed class AdminReportsExportRequestDto
    {
        public string reportId { get; set; }
        public List<string> fields { get; set; }
        public Dictionary<string, string> filterValues { get; set; }

        /// <summary>"en" or "ar" — same normalization rule as preview's `locale` (default "en" for
        /// anything else, never trusted verbatim).</summary>
        public string locale { get; set; }

        /// <summary>Only "excel" is accepted in R4 — PDF is explicitly out of this phase's scope and is
        /// rejected with a clear error, not silently ignored.</summary>
        public string format { get; set; }
    }

    /// <summary>Server-side export artifact — plain data only, no HTTP concept (mirrors
    /// <c>AdminDashboardExportResultDto</c>'s shape; the controller turns this into the actual file
    /// response).</summary>
    public sealed class AdminReportsExportResultDto
    {
        public byte[] content { get; set; }
        public string contentType { get; set; }
        public string fileName { get; set; }
        public int rowCount { get; set; }
    }
}
