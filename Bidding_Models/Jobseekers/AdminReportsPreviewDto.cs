using System.Collections.Generic;

namespace Bidding_Models.Jobseekers
{
    /// <summary>
    /// R2 (Admin Reports) — the generic preview request contract every report type shares. Mirrors the
    /// frontend's `ReportPreviewRequest` (`shared/reports/services/report-builder-api.service.ts`)
    /// field-for-field. `fields`/`filterValues` keys are opaque strings from the client's point of
    /// view — the backend is the only party that decides what they mean (see
    /// `AdminReportsFieldRegistry`); an unknown key is rejected, never treated as a column/expression.
    /// </summary>
    public sealed class AdminReportsPreviewRequestDto
    {
        public string reportId { get; set; }
        public List<string> fields { get; set; }
        public Dictionary<string, string> filterValues { get; set; }
        public int? page { get; set; }
        public int? pageSize { get; set; }

        /// <summary>R3: "en" or "ar" — decides which authoritative label a bilingual field (Governorate,
        /// Specialization) renders as, since the underlying catalog data genuinely has both. Anything
        /// else (missing, unrecognized) defaults to "en" server-side — never trusted beyond that
        /// whitelist of two values. Enum-*value* fields (Employment Status) deliberately do NOT use this:
        /// the backend keeps returning the stable logical key ("employed"/"jobseeker") regardless of
        /// locale, and the frontend renders the localized label — see R3 Section 5.</summary>
        public string locale { get; set; }
    }

    public sealed class AdminReportsColumnDto
    {
        public string key { get; set; }
        public string labelEn { get; set; }
        public string labelAr { get; set; }
    }

    public sealed class AdminReportsSummaryValueDto
    {
        public string key { get; set; }
        public string labelEn { get; set; }
        public string labelAr { get; set; }
        public int value { get; set; }
    }

    /// <summary>One page of report rows. `rows[n]` only ever contains keys from `columns` — never a
    /// raw entity, never a field the caller didn't select/wasn't whitelisted.</summary>
    public sealed class AdminReportsPreviewResultDto
    {
        public int totalCount { get; set; }
        public List<AdminReportsColumnDto> columns { get; set; }
        public List<Dictionary<string, object>> rows { get; set; }
        public List<AdminReportsSummaryValueDto> summary { get; set; }
    }
}
