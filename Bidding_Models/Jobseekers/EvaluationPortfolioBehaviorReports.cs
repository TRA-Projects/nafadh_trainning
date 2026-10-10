using System;
using System.Collections.Generic;
using System.Linq;

namespace Bidding_Models.Jobseekers
{
    /// <summary>Ranks behavior report counts for the job seeker dashboard portlet.</summary>
    public static class EvaluationPortfolioBehaviorReports
    {
        public const int TopCount = 10;

        public const string ConfigGroupCode = "BEHV";

        public static List<JobSeekerEvaluationPortfolioBehaviorReportDto> TakeTop(
            IEnumerable<JobSeekerEvaluationPortfolioBehaviorReportDto> rows)
        {
            if (rows == null)
            {
                return new List<JobSeekerEvaluationPortfolioBehaviorReportDto>();
            }

            return rows
                .Where(row => row != null
                    && row.reportCount > 0
                    && !string.IsNullOrWhiteSpace(row.valueKey))
                .OrderByDescending(row => row.reportCount)
                .ThenBy(row => row.labelEn ?? row.valueKey, StringComparer.OrdinalIgnoreCase)
                .Take(TopCount)
                .ToList();
        }
    }
}
