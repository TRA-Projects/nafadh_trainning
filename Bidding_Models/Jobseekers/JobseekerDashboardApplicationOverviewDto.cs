using System.Collections.Generic;

namespace Bidding_Models.Jobseekers
{
    /// <summary>
    /// Aggregated application analytics for the jobseeker dashboard (not paged).
    /// </summary>
    public class JobseekerDashboardApplicationOverviewDto
    {
        public int totalApplications { get; set; }

        public int applicationsWithAppliedOn { get; set; }

        public int applicationsMissingAppliedOn { get; set; }

        /// <summary>UTC timestamp when the aggregation was computed.</summary>
        public string generatedAtUtc { get; set; }

        public List<JobseekerDashboardApplicationStatusBucketDto> statusBuckets { get; set; }
            = new List<JobseekerDashboardApplicationStatusBucketDto>();

        public List<JobseekerDashboardApplicationActivityPointDto> activityPoints { get; set; }
            = new List<JobseekerDashboardApplicationActivityPointDto>();

        /// <summary>Raw pipeline statuses that did not map to an approved dashboard bucket.</summary>
        public List<JobseekerDashboardApplicationUnknownStatusDto> unknownStatuses { get; set; }
            = new List<JobseekerDashboardApplicationUnknownStatusDto>();
    }

    public class JobseekerDashboardApplicationStatusBucketDto
    {
        public string bucketKey { get; set; }
        public string labelEn { get; set; }
        public string labelAr { get; set; }
        public int count { get; set; }
    }

    public class JobseekerDashboardApplicationActivityPointDto
    {
        /// <summary>Month bucket key (<c>YYYY-MM</c>).</summary>
        public string bucketKey { get; set; }
        public int count { get; set; }
    }

    public class JobseekerDashboardApplicationUnknownStatusDto
    {
        public string pipelineStatus { get; set; }
        public int count { get; set; }
    }
}
