namespace Bidding_Models.Jobseekers
{
    /// <summary>
    /// Link-table codes passed to <see cref="Bidding_Core.Helpers.JobseekerStoredAttachmentHelper"/>.
    /// </summary>
    public static class JobseekerAttachmentLinkCodes
    {
        /// <summary><c>ProfileAttachments</c> (purpose CV).</summary>
        public const string ProfileCv = "CV";

        /// <summary>
        /// <c>ProfileAttachments</c> (purpose EDUCATION_DIPLOMA) — UAT-07 Education Enhancements.
        /// Optional supporting certificate for the General Education Diploma section; one per profile,
        /// same single-attachment-per-profile pattern as <see cref="ProfileCv"/> (parentRecordId
        /// ignored). Entirely separate from the government-sourced diploma data shown alongside it.
        /// </summary>
        public const string EducationDiploma = "EDUCATION_DIPLOMA";

        /// <summary><c>JobSeekerEducationAttachments</c> → <c>JobSeekerEducations</c>.</summary>
        public const string EducationRecord = "EDUCATION";

        /// <summary><c>JobSeekerExperienceAttachments</c> → <c>JobSeekerExperiences</c>.</summary>
        public const string ExperienceRecord = "EXPERIENCE";

        /// <summary><c>JobSeekerCertificateAttachments</c> → <c>JobSeekerCertificates</c>.</summary>
        public const string CertificateRecord = "CERTIFICATE";

        /// <summary><c>JobSeekerTrainingAttachments</c> completion certificate.</summary>
        public const string TrainingCompletion = "TRAINING_COMPLETION";

        /// <summary><c>JobSeekerTrainingAttachments</c> optional supporting file.</summary>
        public const string TrainingOther = "TRAINING_OTHER";

        /// <summary><c>JobSeekerPortfolioItemAttachments</c> → <c>JobSeekerPortfolioItems</c>.</summary>
        public const string PortfolioItem = "PORTFOLIO";
    }
}
