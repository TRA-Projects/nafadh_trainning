using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Bidding_Core.Helpers
{
    public static class NotificationTemplateTokenParser
    {
        public static readonly Regex TokenRegex = new Regex(@"\{([a-z][a-z0-9_]*)\}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string ReplaceTokens(string template, IReadOnlyDictionary<string, string> values)
        {
            if (string.IsNullOrEmpty(template))
            {
                return template ?? string.Empty;
            }

            return TokenRegex.Replace(template, match =>
            {
                var key = match.Groups[1].Value;
                if (values != null && values.TryGetValue(key, out var value))
                {
                    return value ?? string.Empty;
                }

                foreach (var pair in values ?? new Dictionary<string, string>())
                {
                    if (string.Equals(pair.Key, key, System.StringComparison.OrdinalIgnoreCase))
                    {
                        return pair.Value ?? string.Empty;
                    }
                }

                return string.Empty;
            });
        }
    }
}
