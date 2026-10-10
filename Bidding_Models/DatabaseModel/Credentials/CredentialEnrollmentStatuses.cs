namespace Bidding_Models.DatabaseModel.Credentials
{
    /// <summary>
    /// CredentialEnrollment.Status values, exactly as persisted: CredentialEnrollmentDbWriter's
    /// NormalizeStoredEnrollmentStatus maps every write to one of these (an unknown value becomes
    /// Selected), and CredentialResultDbWriter sets Pending Submission / Completed. Mirrors the
    /// frontend EnrollmentStatus type. The legacy input alias "PendingSubmission" is normalized to
    /// <see cref="PendingSubmission"/> on write and is never stored.
    /// </summary>
    public static class CredentialEnrollmentStatuses
    {
        public const string Selected = "Selected";
        public const string Pending = "Pending";
        public const string Scheduled = "Scheduled";
        public const string InProgress = "In Progress";
        public const string PendingSubmission = "Pending Submission";
        public const string Completed = "Completed";
    }
}
