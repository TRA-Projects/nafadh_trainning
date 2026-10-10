namespace Bidding_Models.DatabaseModel.Notifications
{
    public static class CustomNotificationStatuses
    {
        public const string Scheduled = "Scheduled";
        public const string Processing = "Processing";
        public const string Sent = "Sent";
        public const string PartiallyFailed = "PartiallyFailed";
        public const string Failed = "Failed";
        public const string Cancelled = "Cancelled";
    }

    public static class CustomNotificationTargetTypes
    {
        public const string User = "User";
        public const string UserGroup = "UserGroup";
        public const string Organization = "Organization";
        public const string AllOrganizations = "AllOrganizations";
        public const string JobSeekers = "JobSeekers";
    }

    /// <summary>Audience filter for <c>JobSeekers</c> and <c>AllOrganizations</c> targets.</summary>
    public static class CustomNotificationRegistrationFilters
    {
        public const string All = "All";
        public const string Registered = "Registered";
        public const string NotRegistered = "NotRegistered";

        public static string Normalize(string value)
        {
            var v = (value ?? string.Empty).Trim();
            if (string.Equals(v, Registered, System.StringComparison.OrdinalIgnoreCase)) return Registered;
            if (string.Equals(v, NotRegistered, System.StringComparison.OrdinalIgnoreCase)) return NotRegistered;
            return All;
        }

        public static bool IncludesRegistered(string value) => Normalize(value) != NotRegistered;

        public static bool IncludesNotRegistered(string value) => Normalize(value) != Registered;
    }

    public static class CustomNotificationChannels
    {
        public const string Email = "Email";
        public const string Sms = "SMS";
        public const string System = "System";
    }
}
