namespace Bidding_Models.Jobseekers
{
    /// <summary>Stable API <c>Code</c> values for Nafath JobSeeker MOL integration errors.</summary>
    public static class NafathJobSeekerErrorCodes
    {
        public const string FetchFailed = "NAFATH_FETCH_FAILED";
        public const string NoData = "NAFATH_NO_DATA";
        public const string Inactive = "NAFATH_INACTIVE";

        public const string FetchFailedMessage = "Failed to fetch jobseekers data.";
        public const string NoDataMessage = "No jobseeker data found for this civil ID.";
        public const string InactiveMessage = "Civil ID is not registered as an active job seeker.";
    }
}
