namespace Bidding_Models.Jobseekers
{
    /// <summary>Stable API <c>Code</c> values for TRA license verification integration errors.</summary>
    public static class TraLicenseErrorCodes
    {
        public const string FetchFailed = "TRA_LICENSES_FETCH_FAILED";
        public const string FetchFailedMessage = "Failed to fetch TRA license data.";
        public const string LicensedCompaniesFailedMessage = "Failed to fetch TRA licensed companies.";
        public const string ActiveLicensesFailedMessage = "Failed to fetch TRA active licenses.";
        public const string SettingsIncompleteMessage = "TRA license settings are incomplete.";
    }
}
