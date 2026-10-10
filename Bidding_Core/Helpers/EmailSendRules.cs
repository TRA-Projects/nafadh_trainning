namespace Bidding_Core.Helpers
{
    /// <summary>
    /// Outbound mail uses the same SMTP client as merged registration mail.
    /// </summary>
    public static class EmailSendRules
    {
        public const bool SkipOutboundSending = false;

        public static bool ShouldSkipOutboundSending() => SkipOutboundSending;

        /// <summary>Skipped SMTP is treated as success so callers do not roll back.</summary>
        public static bool TreatSkippedSendAsSuccess() => ShouldSkipOutboundSending();
    }
}
