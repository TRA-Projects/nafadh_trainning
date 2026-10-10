namespace Bidding_Models.Jobseekers
{
    /// <summary>
    /// Jobseeker self-service list row — interview appointment metadata only.
    /// Excludes evaluations, ratings, recruiter decisions, internal notes,
    /// other candidates, skill catalogs, and admin/operator metadata.
    /// </summary>
    public class JobseekerInterviewAppointmentListItemDto
    {
        /// <summary>Interview record reference (e.g. IR-5001). Route key for /interviews/{id}/view.</summary>
        public string interviewRecordId { get; set; }

        /// <summary>Enrollment reference (e.g. IE-8001). Stable per jobseeker slot.</summary>
        public string enrollmentId { get; set; }

        /// <summary>Record title (fallback when opportunity title absent).</summary>
        public string titleEn { get; set; }

        public string titleAr { get; set; }

        /// <summary>Opportunity/job title for list display.</summary>
        public string opportunityTitleEn { get; set; }

        public string opportunityTitleAr { get; set; }

        /// <summary>Company owner (from job opportunity owner profile).</summary>
        public string organizationNameEn { get; set; }

        public string organizationNameAr { get; set; }

        /// <summary>On-site, Online, or Phone.</summary>
        public string interviewType { get; set; }

        public string locationOrLink { get; set; }

        /// <summary>Candidate schedule date (yyyy-MM-dd).</summary>
        public string interviewDateIso { get; set; }

        /// <summary>Candidate schedule time (HH:mm).</summary>
        public string interviewTime { get; set; }

        /// <summary>
        /// Interview record workflow status (session-level aggregate).
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// Authenticated jobseeker's enrollment status for this appointment.
        /// </summary>
        public string enrollmentStatus { get; set; }

        /// <summary>
        /// Presentation flag: an earlier enrollment for this record + jobseeker was Rescheduled.
        /// Does not change <see cref="enrollmentStatus"/> (current seat remains Scheduled after reschedule).
        /// </summary>
        public bool wasRescheduled { get; set; }
    }

    /// <summary>
    /// Jobseeker self-service detail — appointment metadata only (no results).
    /// </summary>
    public class JobseekerInterviewAppointmentDetailDto : JobseekerInterviewAppointmentListItemDto
    {
        public string instructionsEn { get; set; }

        public string instructionsAr { get; set; }

        /// <summary>Portal job id for opportunity modal; null when unresolved.</summary>
        public int? relatedJobId { get; set; }

        public System.Collections.Generic.List<SkillRefDto> professionalSkills { get; set; } =
            new System.Collections.Generic.List<SkillRefDto>();

        public System.Collections.Generic.List<SkillRefDto> basicSkills { get; set; } =
            new System.Collections.Generic.List<SkillRefDto>();

        public System.Collections.Generic.List<TechStackRefDto> techStacks { get; set; } =
            new System.Collections.Generic.List<TechStackRefDto>();
    }
}
