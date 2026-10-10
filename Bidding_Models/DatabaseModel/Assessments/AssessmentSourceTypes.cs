namespace Bidding_Models.DatabaseModel.Assessments
{
    /// <summary>Discriminator for <see cref="AssessmentResult.SourceType"/>.</summary>
    public static class AssessmentSourceTypes
    {
        public const string CredentialAssessment = "CredentialAssessment";
        public const string JobPosting = "JobPosting";
        /// <summary>Jobseeker self-rating from profile completion (weight typically 0; excluded from rollup).</summary>
        public const string SelfRating = "SelfRating";
        /// <summary>Employer periodic review from post-employment procedures.</summary>
        public const string PeriodicRating = "PeriodicRating";
        /// <summary>Reserved for future training evaluations (not wired yet).</summary>
        public const string Training = "Training";
    }
}
