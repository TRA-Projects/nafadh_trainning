namespace Bidding_Models.DatabaseModel.JobOpportunities
{
    public static class JobOpportunityStatuses
    {
        public const string Draft = "Draft";
        public const string PendingOperatorAction = "PendingOperatorAction";
        public const string Submitted = "Submitted";
        public const string Published = "Published";
        public const string UnderEvaluation = "UnderEvaluation";
        public const string EmploymentProcess = "EmploymentProcess";
        public const string Withdrawn = "Withdrawn";
        public const string Closed = "Closed";

        public static readonly string[] All =
        {
            Draft,
            PendingOperatorAction,
            Submitted,
            Published,
            UnderEvaluation,
            EmploymentProcess,
            Withdrawn,
            Closed,
        };

        /// <summary>
        /// Opportunities a company has actually offered: live, in-process after publish,
        /// closed, or withdrawn. Excludes draft / pending-operator / submitted (not yet offered).
        /// </summary>
        public static readonly string[] Offered =
        {
            Published,
            UnderEvaluation,
            EmploymentProcess,
            Withdrawn,
            Closed,
        };

        public static bool IsKnown(string status)
            => !string.IsNullOrWhiteSpace(status)
               && System.Array.Exists(All, s => s == status);

        public static bool CountsAsOffered(string status)
            => !string.IsNullOrWhiteSpace(status)
               && System.Array.Exists(Offered, s => s == status);
    }
}
