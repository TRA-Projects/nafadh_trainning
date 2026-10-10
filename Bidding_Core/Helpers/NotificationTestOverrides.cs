namespace Bidding_Core.Helpers
{
    /// <summary>Dev-only routing for outbound email/SMS while testing transport.</summary>
    internal static class NotificationTestOverrides
    {
        /// <summary>When true, all outbound email/SMS go to the fixed test addresses below.</summary>
        internal const bool Enabled = false;
        internal const string Email = "ssam007@hotmail.com";
        internal const string Mobile = "96277631";

        internal static string ResolveEmail(string actual)
        {
            return Enabled ? Email : actual;
        }

        internal static string ResolveMobile(string actual)
        {
            return Enabled ? Mobile : actual;
        }
    }
}
