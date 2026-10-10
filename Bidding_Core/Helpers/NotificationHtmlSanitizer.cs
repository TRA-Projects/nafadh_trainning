using System.Text.RegularExpressions;

namespace Bidding_Core.Helpers
{
    /// <summary>Allowlist sanitizer for admin-authored email HTML.</summary>
    public static class NotificationHtmlSanitizer
    {
        private static readonly Regex ScriptBlock = new Regex(@"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex StyleBlock = new Regex(@"<style\b[^<]*(?:(?!<\/style>)<[^<]*)*<\/style>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex OnEventAttr = new Regex(@"\s(on\w+|style|formaction|xmlns)\s*=\s*(""[^""]*""|'[^']*'|[^\s>]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex JavascriptHref = new Regex(@"\shref\s*=\s*(""javascript:[^""]*""|'javascript:[^']*')", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static string Sanitize(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            var cleaned = ScriptBlock.Replace(html, string.Empty);
            cleaned = StyleBlock.Replace(cleaned, string.Empty);
            cleaned = OnEventAttr.Replace(cleaned, string.Empty);
            cleaned = JavascriptHref.Replace(cleaned, string.Empty);
            return cleaned.Trim();
        }

        public static string StripHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            return Regex.Replace(html, "<[^>]+>", " ").Replace("&nbsp;", " ").Trim();
        }
    }
}
