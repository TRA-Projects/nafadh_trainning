using System.Collections.Generic;

namespace Bidding_Models.Jobseekers.Jobs
{
    /// <summary>
    /// Search-time match request for the job-seekers list.
    /// Provide <see cref="JobId"/> (job-based) or catalog <see cref="SkillIds"/> / <see cref="TechStackIds"/> (advanced).
    /// </summary>
    public class JobSeekerListSearchRequestDto
    {
        public int? JobId { get; set; }

        /// <summary>Professional catalog skill ids (advanced criteria).</summary>
        public List<int> SkillIds { get; set; } = new List<int>();

        /// <summary>Tech-stack catalog skill ids (advanced criteria).</summary>
        public List<int> TechStackIds { get; set; } = new List<int>();
    }
}
